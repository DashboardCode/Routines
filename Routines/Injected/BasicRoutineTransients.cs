using System;

namespace DashboardCode.Routines.Injected
{
    public class BasicRoutineTransients<TStateService> : IRoutineTransients<TStateService>
    {
        readonly IExceptionHandler exceptionHandler;
        readonly IRoutineLogging routineLogging;
        readonly TStateService stateService;

        public BasicRoutineTransients(
            IBasicLogging basicLogging,
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

            var defaultExceptionAdapter = new ExceptionAdapter(
                    basicLogging.LogException, 
                    transformException?? ((e)=> e)
                );

            exceptionHandler = new ExceptionHandler(defaultExceptionAdapter, monitorRoutineDurationTicks);
            if (basicLogging.ShouldBufferVerbose)
            {
                var bufferedVerboseLoggingAdapter = new BufferedVerboseLogging(
                    basicLogging,
                    basicLogging.LogBufferedVerbose,
                    basicLogging.ShouldVerboseWithStackTrace
                    );
                routineLogging = new BufferedRoutineLogging(
                    basicLogging, 
                    bufferedVerboseLoggingAdapter, 
                    bufferedVerboseLoggingAdapter.Flash,
                    testInput,
                    testOutput
                    );
                stateService = createStateService(bufferedVerboseLoggingAdapter.LogVerbose);
            }
            else
            {
                routineLogging = new RoutineLogging(basicLogging, basicLogging);
                stateService = createStateService(basicLogging.LogVerbose);
            }
        }

        public TStateService ResolveStateService() => 
            stateService;

        public IExceptionHandler ResolveExceptionHandler() =>
            exceptionHandler;

        public IRoutineLogging ResolveRoutineLogging() =>
            routineLogging;
    }
}
