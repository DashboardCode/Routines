using System;

namespace DashboardCode.Routines.Injected
{
    public class BasicRoutineTransients<TStateService> : IRoutineTransients<TStateService>
    {
        readonly IExceptionHandler exceptionHandler;
        readonly IRoutineLogging routineLogging;
        readonly TStateService stateService;

        public BasicRoutineTransients(
            IBasicLogging basicRoutineLoggingAdapter,
            Func<Exception, Exception> transformException,
            Func<Action<DateTime, string>, TStateService> createStateService,
            Action<long> monitorRoutineDurationTicks = null,
            Predicate<object> testInput = null,
            Predicate<object> testOutput = null
            )
        {
            if (testInput == null)
                testInput = (o) => false;
            if (testOutput == null)
                testOutput = (o) => false;

            var activityLoggingAdapter = basicRoutineLoggingAdapter;
            var defaultExceptionAdapter = new ExceptionAdapter(
                    basicRoutineLoggingAdapter.LogException, 
                    transformException?? ((e)=> e)
                );

            exceptionHandler = new ExceptionHandler(defaultExceptionAdapter, monitorRoutineDurationTicks);
            if (basicRoutineLoggingAdapter.UseBufferForVerbose)
            {
                var bufferedVerboseLoggingAdapter = new BufferedVerboseLogging(
                    basicRoutineLoggingAdapter,
                    basicRoutineLoggingAdapter.LogBufferedVerbose,
                    basicRoutineLoggingAdapter.VerboseWithStackTrace
                    );
                routineLogging = new BufferedRoutineLogging(
                    activityLoggingAdapter, 
                    bufferedVerboseLoggingAdapter, 
                    bufferedVerboseLoggingAdapter.Flash,
                    testInput,
                    testOutput
                    );
                stateService = createStateService(bufferedVerboseLoggingAdapter.LogVerbose);
            }
            else
            {
                routineLogging = new RoutineLogging(activityLoggingAdapter, basicRoutineLoggingAdapter);
                stateService = createStateService(basicRoutineLoggingAdapter.LogVerbose);
            }
        }

        public TStateService ResolveStateService()
        {
            return stateService;
        }

        public IExceptionHandler ResolveExceptionHandler()
        {
            return exceptionHandler;
        }

        public IRoutineLogging ResolveRoutineLogging()
        {
            return routineLogging;
        }
    }
}
