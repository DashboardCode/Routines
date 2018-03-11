using System;

namespace DashboardCode.Routines.Injected
{
    public class RoutineHandlerFactory<TClosure>
    {
        Action<long> performanceCounter;
        public RoutineHandlerFactory(Action<long> performanceCounter) =>
            this.performanceCounter = performanceCounter;

        public (IRoutineHandler<TClosure>, TClosure) CreateRoutineHandler(
                bool shouldBufferVerbose,
                Func<Action<DateTime, string>, TClosure> createClosure,
                ExceptionHandler exceptionHandler,
                bool finishActivity, 
                object input,
                IActivityLogger activityLogger,
                IDataLogger dataLogger,
                IBufferedVerboseLogger bufferedVerboseLogger,
                VerboseBuffer buffer,
                bool shouldVerboseWithStackTrace,
                Func<object, object, TimeSpan, bool> testInputOutput
            )
        {
            IRoutineHandler<TClosure> routineHandler = null;
            TClosure closure = default;
            if (shouldBufferVerbose)
            {
                var (start, logVerbose) = CreateRoutineHandlerVerbose(
                    createClosure,
                    finishActivity,
                    performanceCounter,
                    input,
                    activityLogger,
                    dataLogger,
                    bufferedVerboseLogger,
                    buffer,
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
                    dataLogger
                );
                closure = createClosure(null/*logVerbose*/);
                routineHandler = new RoutineHandlerSilent<TClosure>(closure, exceptionHandler, silentStart);
            };
            return (routineHandler, closure);
        }

        private static (Func<(Action<object>, Action)>, Action<DateTime, string>) CreateRoutineHandlerVerbose(
                Func<Action<DateTime, string>, TClosure> createClosure,
                bool finishActivity,
                Action<long> performanceCounter,
                object input,
                IActivityLogger activityLogger,
                IDataLogger dataLogger,
                IBufferedVerboseLogger bufferedVerboseLogger,
                VerboseBuffer buffer,
                bool shouldVerboseWithStackTrace,
                Func<object, object, TimeSpan, bool> testInputOutput
            )
        {
            var buffered = new BufferedVerboseLogging(
                    buffer,
                    dataLogger,
                    bufferedVerboseLogger.LogBufferedVerbose,
                    shouldVerboseWithStackTrace
                );
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
                            dataLogger.Input(startDateTime, input);
                            dataLogger.Output(DateTime.Now, output);
                            buffered.Flash();
                        }
                    };
                    Action onFailure = () =>
                    {
                        onFinish();
                        dataLogger.Input(startDateTime, input);
                        buffered.Flash();
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
                            dataLogger.Input(startDateTime, input);
                            dataLogger.Output(DateTime.Now, output);
                            buffered.Flash();
                        }
                    };
                    Action onFailure = () =>
                    {
                        onFinish(false);
                        dataLogger.Input(startDateTime, input);
                        buffered.Flash();
                    };
                    return (onSuccess, onFailure);
                };
            }
            return (logOnStart, buffered.LogVerbose);
        }

        private static Func<(Action, Action)> CreateRoutineHandlerSilent(
                Func<Action<DateTime, string>, TClosure> createClosure,
                bool finishActivity,
                Action<long> performanceCounter,
                object input,
                IActivityLogger activityLogger,
                IDataLogger dataLogger
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
                        dataLogger.Input(startDateTime, input);
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
                        dataLogger.Input(startDateTime, input);
                        onFinish(false);
                    };
                    return (() => onFinish(true), onFailure);
                };
            }
            return logOnStart;
        }
    }
}