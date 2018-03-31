namespace DashboardCode.AdminkaV1.Injected.Diagnostics
{
    public interface IPerformanceCounters
    {
        void CountDurationTicks(long ticks);
        void CountError();
    }
}