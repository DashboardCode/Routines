using System;

namespace DashboardCode.Routines.Injected
{
    public class RoutineLogging : IRoutineLogging
    {
        private readonly IDataLogger dataLogging;
        private readonly ActivityState activityState;
        public RoutineLogging(
            ActivityState activityState,
            IDataLogger dataLogging
            )
        {
            this.activityState = activityState;
            this.dataLogging = dataLogging;
        }
        public (Action<object>, Action) LogStart(object input)
        {
            var onFinish = activityState.LogStart();
            dataLogging.Input(DateTime.Now, input);
            Action<object> onSuccess = (object output) =>
            {
                dataLogging.Output(DateTime.Now, output);
                onFinish(true);
            };
            Action onFailure = () => onFinish(false);
            return (onSuccess, onFailure);
        }
    }
}