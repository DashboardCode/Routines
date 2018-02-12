using System;

namespace DashboardCode.Routines.Injected
{
    public class ActivityState
    {
        private readonly IActivityLogger activityLogger;

        public ActivityState(IActivityLogger activityLogger) =>
            this.activityLogger = activityLogger;
        
        public Action<bool> LogStart()
        {
            var startDateTime = DateTime.Now;
            activityLogger.LogActivityStart(startDateTime);
            return (isSuccess) =>
            {
                var finnishDateTime = DateTime.Now;
                var duration = finnishDateTime - startDateTime;
                var seconds = (decimal)duration.Milliseconds / 1000;
                activityLogger.LogActivityFinish(finnishDateTime, duration, isSuccess);
            };
        }
    }
}