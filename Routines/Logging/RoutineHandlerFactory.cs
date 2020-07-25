using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Logging
{
    public class RoutineHandlerFactory
    {
        public readonly Guid CorrelationToken;
        private readonly List<VerboseBuffer> buffers = new List<VerboseBuffer>();
        public RoutineHandlerFactory(Guid correlationToken)
        {
            CorrelationToken = correlationToken;
        }

        public void Flash()
        {
            foreach (var b in buffers)
                b.Flash();
        }

        public IMemberLogger CreateMemberLogger(IMemberLogger memberLogger, bool shouldVerboseWithStackTrace, bool startActivity)
        {
            var buffer = new VerboseBuffer(memberLogger.LogBufferedVerbose, shouldVerboseWithStackTrace);
            buffers.Add(buffer);
            return new BufferedMemberLogger(memberLogger, buffer.LogVerbose, startActivity);
        }

        public (IHandlerOmni<TClosure>, TClosure) CreateRoutineHandler<TClosure>(
                bool veroseEnabled,
                Func<Action<DateTime, string>, TClosure> createClosure,
                IExceptionHandler exceptionHandler,
                //Func<Exception, Exception> transformException,
                //Action<Exception> handleException,
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
            IHandlerOmni<TClosure> routineHandler = null;
            TClosure closure = default;
            //var  exceptionHandler = (transformException == null) ? 
            //    new ExceptionAbsorbHandler(handleException)  : new ExceptionHandler(handleException, transformException) as IExceptionHandler;
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
                    memberTag,
                    logVerbose,
                    this.Flash,
                    shouldVerboseWithStackTrace,
                    testInputOutput
                );
                closure = createClosure(logVerbose);
                var handlerVerbose = new HandlerVerbose<TClosure>(closure, exceptionHandler, start);
                routineHandler = handlerVerbose;

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
                var handlerSilent = new HandlerSilent<TClosure>(closure, exceptionHandler, silentStart);
                routineHandler = handlerSilent;
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

    }
}