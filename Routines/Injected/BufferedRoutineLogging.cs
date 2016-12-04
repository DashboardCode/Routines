using System;

namespace Vse.Routines.Injected
{
    public class BufferedRoutineLogging : IRoutineLogging
    {
        private readonly IVerboseLogging verboseLoggingAdapter;
        private readonly Action flashBuffer;
        private readonly Predicate<object> testInput;
        private readonly Predicate<object> testOutput;
        private readonly ActivityStateLogger activityLogger;

        public BufferedRoutineLogging(
            IActivityLogging activityLoggingAdapter,
            IVerboseLogging verboseLoggingAdapter,
            Action flashBuffer,
            Predicate<object> testInput,
            Predicate<object> testOutput
            )
        {
            this.verboseLoggingAdapter = verboseLoggingAdapter;
            this.flashBuffer = flashBuffer;
            this.testInput = testInput;
            this.testOutput = testOutput;

            activityLogger = new ActivityStateLogger(
                activityLoggingAdapter
            );
        }

        public void LogStart(object input)
        {
            activityLogger.LogStart();
            verboseLoggingAdapter.Input(DateTime.Now, input);
        }

        public void LogFinish(bool isSuccess, object output)
        {
            verboseLoggingAdapter.Output(DateTime.Now, output);
            if (!isSuccess || !testOutput(output) )
            {
                flashBuffer();
            }
            activityLogger.LogFinish(isSuccess);
        }
    }
}