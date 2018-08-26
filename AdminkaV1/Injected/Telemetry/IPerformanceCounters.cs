namespace DashboardCode.AdminkaV1.Injected.Telemetry
{
    public interface IPerformanceCounters
    {
        void CountDurationTicks(long ticks);
        void CountError();
    }
}