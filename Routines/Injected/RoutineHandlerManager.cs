using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardCode.Routines.Injected
{
    public static class RoutineHandlerManager
    {
        public static (IRoutineHandler<TClosure>, TClosure) CreateRoutineHandler<TClosure>(
                ExceptionHandler exceptionHandler,
                //MemberTag memberTag,
                //IContainer container,
                Func<Action<DateTime, string>, TClosure> createClosure,
                bool shouldBufferVerbose,
                bool finishActivity, // loggingConfiguration.FinishActivity
                Func<object, object, TimeSpan, bool> testInputOutput,
                Action<long> performanceCounter,
                IActivityLogger activityLogger,
                IDataLogger dataLogger,
                object input,
                //UserContext userContext,
                IExceptionLogger exceptionLogger,
                VerboseBuffer buffer,
                bool shouldVerboseWithStackTrace, // loggingVerboseConfiguration.ShouldVerboseWithStackTrace
                IBufferedVerboseLogger bufferedVerboseLogger
            )
        {
            IRoutineHandler<TClosure> routineHandler = null;
            TClosure closure = default;
            if (!shouldBufferVerbose)
            {
                Func<(DateTime, Action<bool>)> startSilent;
                if (finishActivity == false)
                {
                    var activityState = new ActivityStatePerformanceCounterOnly(performanceCounter);
                    startSilent = activityState.StartSilent;
                }
                else
                {
                    var activityState = new ActivityState(activityLogger.LogActivityStart, activityLogger.LogActivityFinish, performanceCounter);
                    startSilent = activityState.StartSilent;
                }
                var routineLogging = new RoutineLogging(startSilent, dataLogger);
                Func<(Action, Action)> logOnStart = routineLogging.Compose(input);
                //closure = new RoutineClosure<UserContext>(userContext, null /*logVerbose*/, container);
                closure = createClosure(null /*logVerbose*/);
                routineHandler = new RoutineHandlerSilent<TClosure>(closure, exceptionHandler, logOnStart);
            }
            else
            {
                Func<(DateTime, Func<bool, TimeSpan>)> start;
                if (finishActivity == false)
                {
                    var activityState = new ActivityStatePerformanceCounterOnly(performanceCounter);
                    start = activityState.Start;
                }
                else
                {
                    var activityState = new ActivityState(activityLogger.LogActivityStart, activityLogger.LogActivityFinish, performanceCounter);
                    start = activityState.Start;
                }
                var buffered = new BufferedVerboseLogging(
                    buffer,
                    dataLogger,
                    bufferedVerboseLogger.LogBufferedVerbose,
                    shouldVerboseWithStackTrace
                );
                Action<DateTime, string> logVerbose = buffered.LogVerbose;
                var bufferedRoutineLogging = new BufferedRoutineLogging(
                    start,
                    buffered,
                    logVerbose,
                    buffered.Flash,
                    testInputOutput);
                Func<(Action<object>, Action)> logOnStart = bufferedRoutineLogging.Compose(input);
                closure = createClosure(logVerbose);
                //closure = new RoutineClosure<UserContext>(userContext, logVerbose, container);
                routineHandler = new RoutineHandler<TClosure>(closure, exceptionHandler, logOnStart);
            };
            return (routineHandler, closure);
        }
    }
}
