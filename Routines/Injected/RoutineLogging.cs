using System;

namespace DashboardCode.Routines.Injected
{
    public class RoutineLogging : IRoutineLogging
    {
        private readonly IVerboseLogging verboseLoggingAdapter;
        private readonly ActivityStateLogger activityLoggingFacade;
        public RoutineLogging(
            IActivityLogging activityLoggingAdapter,
            IVerboseLogging verboseLoggingAdapter
            )
        {
            this.verboseLoggingAdapter = verboseLoggingAdapter;
            activityLoggingFacade = new ActivityStateLogger(activityLoggingAdapter);
        }
        public void LogStart(object input)
        {
            activityLoggingFacade.LogStart();
            verboseLoggingAdapter.Input(DateTime.Now, input);
        }
        public void LogFinish(bool isSuccess, object output)
        {
            verboseLoggingAdapter.Output(DateTime.Now, output);
            activityLoggingFacade.LogFinish(isSuccess);
        }
    }
}