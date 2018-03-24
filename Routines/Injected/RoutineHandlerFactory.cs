using System;

namespace DashboardCode.Routines.Injected
{
    public class RoutineHandlerFactory<TClosure>
    {
        Action<long> performanceCounter;
        public RoutineHandlerFactory(Action<long> performanceCounter) =>
            this.performanceCounter = performanceCounter;

        public (IRoutineHandler<TClosure>, TClosure) CreateRoutineHandler(
                bool veroseEnabled,
                Func<Action<DateTime, string>, TClosure> createClosure,
                ExceptionHandler exceptionHandler,
                bool finishActivity,
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
            IRoutineHandler<TClosure> routineHandler = null;
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
                    flash,
                    shouldVerboseWithStackTrace,
                    testInputOutput
                );
                closure = createClosure(logVerbose);
                routineHandler = new RoutineHandlerVerbose<TClosure>(closure, exceptionHandler, start);
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
                routineHandler = new RoutineHandlerSilent<TClosure>(closure, exceptionHandler, silentStart);
            };
            return (routineHandler, closure);
        }

        private static Func<(Action<object>, Action)> CreateRoutineHandlerVerbose(
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
                    Func<TimeSpan> onFinish = () =>
                    {
                        var finnishDateTime = DateTime.Now;
                        var duration = finnishDateTime - startDateTime;
                        performanceCounter(duration.Ticks);
                        return duration;
                    };
                    Action<object> onSuccess = output =>
                    {
                        var duration = onFinish();
                        if (testInputOutput(input, output, duration))
                        {
                            logInput(startDateTime, input);
                            logOutput(DateTime.Now, output);
                            flash();
                        }
                    };
                    Action onFailure = () =>
                    {
                        onFinish();
                        logInput(startDateTime, input);
                        flash();
                    };
                    return (onSuccess, onFailure);
                };
            }
            else
            {
                logOnStart = () =>
                {
                    var startDateTime = DateTime.Now;
                    activityLogger.LogActivityStart(startDateTime);

                    Func<bool, TimeSpan> onFinish = isSuccess =>
                    {
                        var finishDateTime = DateTime.Now;
                        var duration = finishDateTime - startDateTime;
                        activityLogger.LogActivityFinish(finishDateTime, duration, isSuccess);
                        performanceCounter(duration.Ticks);
                        return duration;
                    };
                    Action<object> onSuccess = (output) =>
                    {
                        var duration = onFinish(true);
                        if (testInputOutput(input, output, duration))
                        {
                            logInput(startDateTime, input);
                            logOutput(DateTime.Now, output);
                            flash();
                        }
                    };
                    Action onFailure = () =>
                    {
                        onFinish(false);
                        logInput(startDateTime, input);
                        flash();
                    };
                    return (onSuccess, onFailure);
                };
            }
            return logOnStart;
        }

        private static Func<(Action, Action)> CreateRoutineHandlerSilent(
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
                    Action onFinish = () =>
                    {
                        var duration = DateTime.Now - startDateTime;
                        performanceCounter(duration.Ticks);
                    };
                    Action onFailure = () =>
                    {
                        logInput(startDateTime, input);
                        onFinish();
                    };
                    return (onFinish, onFailure);
                };
            }
            else
            {
                logOnStart = () =>
                {
                    var startDateTime = DateTime.Now;
                    activityLogger.LogActivityStart(startDateTime);

                    Action<bool> onFinish = isSuccess =>
                    {
                        var finishDateTime = DateTime.Now;
                        var duration = finishDateTime - startDateTime;
                        activityLogger.LogActivityFinish(finishDateTime, duration, isSuccess);
                        performanceCounter(duration.Ticks);
                    };
                    Action onFailure = () =>
                    {
                        logInput(startDateTime, input);
                        onFinish(false);
                    };
                    return (() => onFinish(true), onFailure);
                };
            }
            return logOnStart;
        }
    }
}