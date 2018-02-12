using System;

namespace DashboardCode.Routines.Injected
{
    public class BasicRoutineTransientsFactory<TClosure>
    {
        bool shouldBufferVerbose;
        bool shouldVerboseWithStackTrace;
        RoutineLogger routineLogger;
        IBasicLogging basicLogging;
        Predicate<object> testInput = null;
        Predicate<object> testOutput = null;

        public BasicRoutineTransientsFactory(
            bool shouldBufferVerbose,
            bool shouldVerboseWithStackTrace,
            RoutineLogger routineLogger,
            IBasicLogging basicLogging,
            Predicate<object> testInput = null,
            Predicate<object> testOutput = null
            )
        {
            this.shouldBufferVerbose = shouldBufferVerbose;
            this.shouldVerboseWithStackTrace = shouldVerboseWithStackTrace;
            this.routineLogger = routineLogger;
            this.basicLogging = basicLogging;

            this.testInput = testInput;
            this.testOutput = testOutput;
        }

        public IRoutineLogging Create()
        {
            if (testInput == null)
                testInput = (o) => false;
            if (testOutput == null)
                testOutput = (o) => false;

            IRoutineLogging routineLogging = null;
            if (shouldBufferVerbose)
            {
                var bufferedVerboseLoggingAdapter = new BufferedVerboseLogging(
                    basicLogging,
                    basicLogging.LogBufferedVerbose,
                    shouldVerboseWithStackTrace,
                    routineLogger
                    );
                routineLogging = new BufferedRoutineLogging(
                    basicLogging,
                    bufferedVerboseLoggingAdapter,
                    bufferedVerboseLoggingAdapter.Flash,
                    testInput,
                    testOutput
                    );
            }
            else
            {
                var activityStateLogger = new ActivityState(basicLogging);
                routineLogging = new RoutineLogging(activityStateLogger, basicLogging);
            }
            return routineLogging;
        }
    }
}