using System;

namespace DashboardCode.Routines.Injected
{
    public class BufferedRoutineLogging : IRoutineLogging
    {
        private readonly IDataLogger verboseLoggingAdapter;
        private readonly Action flashBuffer;
        private readonly Predicate<object> testInput;
        private readonly Predicate<object> testOutput;
        private readonly ActivityState activityLogger;

        public BufferedRoutineLogging(
            IActivityLogger activityLoggingAdapter,
            IDataLogger verboseLoggingAdapter,
            Action flashBuffer,
            Predicate<object> testInput,
            Predicate<object> testOutput
            )
        {
            this.verboseLoggingAdapter = verboseLoggingAdapter;
            this.flashBuffer = flashBuffer;
            this.testInput = testInput;
            this.testOutput = testOutput;

            activityLogger = new ActivityState(
                activityLoggingAdapter
            );
        }

        public (Action<object>, Action) LogStart(object input)
        {
            var logOnFinish = activityLogger.LogStart();
            verboseLoggingAdapter.Input(DateTime.Now, input);
            Action<object> logOnSuccess = (output) =>
            {
                verboseLoggingAdapter.Output(DateTime.Now, output);
                if (!testOutput(output))
                    flashBuffer();
                logOnFinish(true);
            };
            Action logOnFailure = () =>
            {
                flashBuffer();
                logOnFinish(false);
            };
            return (logOnSuccess, logOnFailure);
        }
    }
}