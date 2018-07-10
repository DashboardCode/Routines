using NLog.Conditions;

namespace DashboardCode.NLogTools
{
    [ConditionMethods]
    public static class NLogExtensions
    {
        readonly static PerDayСounter PerDayСounter = new PerDayСounter();

        [ConditionMethod("daycount")]
        public static long Count() =>
            PerDayСounter.Count();
    }
}