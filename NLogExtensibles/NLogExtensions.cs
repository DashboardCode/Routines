using NLog.Conditions;

namespace DashboardCode.NLogExtensibles
{
    [ConditionMethods]
    public static class NLogExtensions
    {
        readonly static PerDayСounter PerDayСounter=new PerDayСounter();

        [ConditionMethod("daycount")]
        public static long Count()
        {
            return PerDayСounter.Count();
        }
    }
}