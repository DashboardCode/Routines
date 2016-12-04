using System;

namespace Vse.Routines.Injected
{
    public class ActivityStateLogger
    {
        private readonly IActivityLogging activityLoggingAdapter;
        private DateTime startDateTime;

        public ActivityStateLogger(IActivityLogging activityLoggingAdapter)
        {
            this.activityLoggingAdapter = activityLoggingAdapter;
        }
        
        public void LogStart()
        {
            startDateTime = DateTime.Now;
            activityLoggingAdapter.LogActivityStart(startDateTime);
        }

        public void LogFinish(bool isSuccess)
        {
            var finnishDateTime = DateTime.Now;
            var duration = finnishDateTime - startDateTime;
            var seconds = (decimal)duration.Milliseconds / 1000;
            activityLoggingAdapter.LogActivityFinish(finnishDateTime, duration, isSuccess);
        }
    }
}
