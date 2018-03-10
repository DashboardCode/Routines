using System;

namespace DashboardCode.Routines.Injected
{
    //public interface IActivityState{
    //    (DateTime, Func<bool, TimeSpan>) Start();
    //    (DateTime, Action<bool>) StartSilent();
    //}

    public class ActivityState //: IActivityState
    {
        Action<DateTime> logActivityStart;
        Action<DateTime, TimeSpan, bool> logActivityFinish;
        Action<long> performaceCounter;

        public ActivityState(
              Action<DateTime> logActivityStart,
              Action<DateTime, TimeSpan, bool> logActivityFinish,
              Action<long> performaceCounter
            ) 
        {
            this.logActivityStart  = logActivityStart;
            this.logActivityFinish = logActivityFinish;
            this.performaceCounter=performaceCounter;
    }
        
        public (DateTime, Func<bool, TimeSpan>) Start()
        {
            var startDateTime = DateTime.Now;
            logActivityStart(startDateTime);

            Func<bool, TimeSpan> onFinish = isSuccess =>
            {
                var finnishDateTime = DateTime.Now;
                var duration = finnishDateTime - startDateTime;
                logActivityFinish(finnishDateTime, duration, isSuccess);
                performaceCounter(duration.Ticks);
                return duration;
            };
            return (startDateTime, onFinish);
        }

        public (DateTime, Action<bool>) StartSilent()
        {
            var startDateTime = DateTime.Now;
            logActivityStart(startDateTime);

            Action<bool> onFinish = isSuccess =>
            {
                var finnishDateTime = DateTime.Now;
                var duration = finnishDateTime - startDateTime;
                logActivityFinish(finnishDateTime, duration, isSuccess);
                performaceCounter(duration.Ticks);
            };
            return (startDateTime, onFinish);
        }
    }

    public class ActivityStatePerformanceCounterOnly //: IActivityState
    {
        Action<long> performanceCounter;
        public ActivityStatePerformanceCounterOnly(Action<long> performanceCounter) =>
            this.performanceCounter = performanceCounter;

        public (DateTime, Func<bool, TimeSpan>) Start()
        {
            var startDateTime = DateTime.Now;
            Func<bool, TimeSpan> onFinish = isSuccess =>
            {
                var finnishDateTime = DateTime.Now;
                var duration = finnishDateTime - startDateTime;
                var ticks = duration.Ticks;
                performanceCounter(ticks);
                return duration;
            };
            return (startDateTime, onFinish);
        }

        public (DateTime, Action<bool>) StartSilent()
        {
            var startDateTime = DateTime.Now;
            Action<bool> onFinish = isSuccess =>
            {
                var finnishDateTime = DateTime.Now;
                var duration = finnishDateTime - startDateTime;
                var ticks = duration.Ticks;
                performanceCounter(ticks);
            };
            return (startDateTime, onFinish);
        }
    }
}