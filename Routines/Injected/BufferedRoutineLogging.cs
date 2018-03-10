using System;

namespace DashboardCode.Routines.Injected
{
    public class BufferedRoutineLogging 
    {
        private readonly IDataLogger dataLogger;
        private readonly Action<DateTime, string> logVerbose;
        private readonly Func<object, object, TimeSpan, bool> testInputOutput;
        private readonly Func<(DateTime, Func<bool, TimeSpan>)> start;
        private readonly Action flash;

        public BufferedRoutineLogging(
            Func<(DateTime, Func<bool, TimeSpan>)> start,
            IDataLogger dataLogger,
            Action<DateTime, string> logVerbose,
            Action flash,
            Func<object, object, TimeSpan, bool> testInputOutput
            )
        {
            this.start = start;
            this.dataLogger = dataLogger;
            this.logVerbose = logVerbose;
            this.flash = flash;
            this.testInputOutput = testInputOutput;
        }

        public Func<(Action<object>, Action)> Compose(object input)
        {
            return () =>
            {
                var (startDateTime, onFinish) = start();
                Action<object> onOutput = (output) =>
                {
                    var duration = onFinish(true);
                    if (testInputOutput(input, output, duration)) 
                    {
                        dataLogger.Input(startDateTime, input);
                        dataLogger.Output(DateTime.Now, output);
                        flash();
                    }
                };
                Action onFailure = () =>
                {
                    onFinish(false);
                    dataLogger.Input(startDateTime, input);
                    flash();
                };
                return (onOutput, onFailure);
            };
        }
    }
}