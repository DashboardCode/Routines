using System;

namespace DashboardCode.Routines.Injected
{
    public class RoutineLogging 
    {
        private readonly IDataLogger dataLogging;
        private readonly Func<(DateTime, Action<bool>)> startSilent;
        public RoutineLogging(
            Func<(DateTime, Action<bool>)> startSilent,
            IDataLogger dataLogging
            )
        {
            this.startSilent = startSilent;
            this.dataLogging   = dataLogging;
        }

        public Func<(Action, Action)> Compose(object input)
        {
            return () =>
            {
                var (startDateTime, onFinish) = startSilent();
                Action onOutput = () =>
                    onFinish(true); 
                Action onFailure = () => {
                    dataLogging.Input(startDateTime, input);
                    onFinish(false);
                };
                return (onOutput, onFailure);
            };
        }
    }
}