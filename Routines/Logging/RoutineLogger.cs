using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Logging
{
    public class RoutineLogger
    {
        public readonly Guid CorrelationToken;
        private readonly List<VerboseBuffer> buffers = new List<VerboseBuffer>();
        //private readonly Func<Guid, MemberTag, IMemberLogger> createMemberLogger;
        public RoutineLogger(Guid correlationToken /*, Func<Guid, MemberTag,  IMemberLogger> createMemberLogger*/)
        {
            CorrelationToken = correlationToken;
            //this.createMemberLogger = createMemberLogger;
        }

        public (IHandler<TClosure>, TClosure) CreateRoutineHandler<TClosure>(
                bool veroseEnabled,
                Func<Action<DateTime, string>, TClosure> createClosure,
                ExceptionHandler exceptionHandler,
                bool finishActivity,
                object input,
                IActivityLogger activityLogger,
                Action<DateTime, object> logInput,
                Action<DateTime, object> logOutput,
                MemberTag memberTag,
                Action<DateTime, string> logVerbose,
                bool shouldVerboseWithStackTrace,
                Func<object, object, TimeSpan, bool> testInputOutput,
                Action<long> performanceCounter
            )
        {
            IHandler<TClosure> routineHandler = null;
            TClosure closure = default;
            if (veroseEnabled)
            {
                var start = CreateRoutineHandlerVerbose(
                    createClosure,
                    finishActivity,
                    performanceCounter,
                    input,
                    activityLogger,
                    logInput,
                    logOutput,
                    //bufferedVerboseLogger,
                    memberTag,
                    logVerbose,
                    this.Flash,
                    shouldVerboseWithStackTrace,
                    testInputOutput
                );
                closure = createClosure(logVerbose);
                routineHandler = new HandlerVerbose<TClosure>(closure, exceptionHandler, start);
            }
            else
            {
                var silentStart = CreateRoutineHandlerSilent(
                    createClosure,
                    finishActivity,
                    performanceCounter,
                    input,
                    activityLogger,
                    logInput
                );
                closure = createClosure(null/*logVerbose*/);
                routineHandler = new HandlerSilent<TClosure>(closure, exceptionHandler, silentStart);
            };
            return (routineHandler, closure);
        }

        private static Func<(Action<object>, Action)> CreateRoutineHandlerVerbose<TClosure>(
                Func<Action<DateTime, string>, TClosure> createClosure,
                bool finishActivity,
                Action<long> performanceCounter,
                object input,
                IActivityLogger activityLogger,
                Action<DateTime, object> logInput,
                Action<DateTime, object> logOutput,
                //IBufferedVerboseLogger bufferedVerboseLogger,
                MemberTag memberTag,
                Action<DateTime, string> logVerbose,
                Action flash,
                bool shouldVerboseWithStackTrace,
                Func<object, object, TimeSpan, bool> testInputOutput
            )
        {
            Func<(Action<object>, Action)> logOnStart;
            if (finishActivity == false)
            {
                logOnStart = () =>
                {
                    var startDateTime = DateTime.Now;
                    TimeSpan onFinish()
                    {
                        var finnishDateTime = DateTime.Now;
                        var duration = finnishDateTime - startDateTime;
                        performanceCounter(duration.Ticks);
                        return duration;
                    }
                    void onSuccess(object output)
                    {
                        var duration = onFinish();
                        if (testInputOutput(input, output, duration))
                        {
                            logInput(startDateTime, input);
                            logOutput(DateTime.Now, output);
                            flash();
                        }
                    }
                    void onFailure()
                    {
                        onFinish();
                        logInput(startDateTime, input);
                        flash();
                    }
                    return (onSuccess, onFailure);
                };
            }
            else
            {
                logOnStart = () =>
                {
                    var startDateTime = DateTime.Now;
                    activityLogger.LogActivityStart(startDateTime);

                    TimeSpan onFinish(bool isSuccess)
                    {
                        var finishDateTime = DateTime.Now;
                        var duration = finishDateTime - startDateTime;
                        activityLogger.LogActivityFinish(finishDateTime, duration, isSuccess);
                        performanceCounter(duration.Ticks);
                        return duration;
                    }
                    void onSuccess(object output)
                    {
                        var duration = onFinish(true);
                        if (testInputOutput(input, output, duration))
                        {
                            logInput(startDateTime, input);
                            logOutput(DateTime.Now, output);
                            flash();
                        }
                    }
                    void onFailure()
                    {
                        onFinish(false);
                        logInput(startDateTime, input);
                        flash();
                    }
                    return (onSuccess, onFailure);
                };
            }
            return logOnStart;
        }

        private static Func<(Action onSuccess, Action onFailre)> CreateRoutineHandlerSilent<TClosure>(
                Func<Action<DateTime, string>, TClosure> createClosure,
                bool finishActivity,
                Action<long> performanceCounter,
                object input,
                IActivityLogger activityLogger,
                Action<DateTime, object> logInput
            )
        {
            Func<(Action, Action)> logOnStart;
            if (finishActivity == false)
            {
                logOnStart = () =>
                {
                    var startDateTime = DateTime.Now;
                    void onFinish()
                    {
                        var duration = DateTime.Now - startDateTime;
                        performanceCounter(duration.Ticks);
                    }
                    void onFailure()
                    {
                        logInput(startDateTime, input);
                        onFinish();
                    }
                    return (onFinish, onFailure);
                };
            }
            else
            {
                logOnStart = () =>
                {
                    var startDateTime = DateTime.Now;
                    activityLogger.LogActivityStart(startDateTime);

                    void onFinish(bool isSuccess)
                    {
                        var finishDateTime = DateTime.Now;
                        var duration = finishDateTime - startDateTime;
                        activityLogger.LogActivityFinish(finishDateTime, duration, isSuccess);
                        performanceCounter(duration.Ticks);
                    }
                    void onFailure()
                    {
                        logInput(startDateTime, input);
                        onFinish(false);
                    }
                    return (() => onFinish(true), onFailure);
                };
            }
            return logOnStart;
        }

        public void Flash()
        {
            foreach (var b in buffers)
                b.Flash();
        }
        
        public IMemberLogger CreateMemberLogger(IMemberLogger memberLogger/*MemberTag memberTag*/, bool shouldVerboseWithStackTrace, bool startActivity)
        {
            //var memberLogger = createMemberLogger(CorrelationToken, memberTag);
            var buffer = new VerboseBuffer(memberLogger.LogBufferedVerbose, shouldVerboseWithStackTrace);
            buffers.Add(buffer);
            return new BufferedMemberLogger(memberLogger, buffer.LogVerbose, startActivity);
        }

        class BufferedMemberLogger : IMemberLogger
        {
            readonly IMemberLogger memberLogger;
            readonly Action<DateTime, string> logVerbose;
            readonly bool startActivity;

            public BufferedMemberLogger(IMemberLogger memberLogger, Action<DateTime,string> logVerbose, bool startActivity)
            {
                this.memberLogger = memberLogger;
                this.logVerbose = logVerbose;
                this.startActivity = startActivity;
            }

            public void Input(DateTime dateTime, object input)
            {
                memberLogger.Input(dateTime, input);
            }

            public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
            {
                memberLogger.LogActivityFinish(dateTime, timeSpan, isSuccess);
            }

            public void LogActivityStart(DateTime dateTime)
            {
                if (startActivity)
                    memberLogger.LogActivityStart(dateTime);
            }

            public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
            {
                memberLogger.LogBufferedVerbose(verboseMessages);
            }

            public void LogError(DateTime dateTime, string message)
            {
                memberLogger.LogError(dateTime, message);
            }

            public void LogException(DateTime dateTime, Exception exception)
            {
                memberLogger.LogException(dateTime, exception);
            }

            public void LogVerbose(DateTime dateTime, string message)
            {
                logVerbose(dateTime, message);
            }

            public void Output(DateTime dateTime, object output)
            {
                memberLogger.Output(dateTime, output);
            }
        }
    }
}