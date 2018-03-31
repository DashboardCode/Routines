using System.Diagnostics;

namespace DashboardCode.AdminkaV1.Injected.Diagnostics
{
    public class PerformanceCounters : IPerformanceCounters
    {
        PerformanceCounter averageActionCounter;
        PerformanceCounter averageActionBaseCounter;
        PerformanceCounter numberOfActionsCounter;
        PerformanceCounter errorCounter;

        public PerformanceCounters(string categoryName, string instanceName)
        {
            averageActionCounter = new PerformanceCounter(categoryName, "Avg. sec/action", instanceName, false);
            averageActionBaseCounter = new PerformanceCounter(categoryName, "Avg. sec/action base", instanceName, false);
            numberOfActionsCounter = new PerformanceCounter(categoryName, "number of actions", instanceName, false);
            errorCounter = new PerformanceCounter(categoryName, "Errors", instanceName, false);
        }

        public void CountDurationTicks(long ticks)
        {
            averageActionCounter.IncrementBy(ticks);
            averageActionBaseCounter.Increment();
            numberOfActionsCounter.Increment();
        }

        public void CountError()
        {
            errorCounter.Increment();
        }
    }
}