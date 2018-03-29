using System;
using System.Collections.Generic;
using System.Text;

namespace DashboardCode.AdminkaV1.Injected.Diagnostics
{
    public interface IPerformanceCounters
    {
        void CountDurationTicks(long ticks);
        void CountError();
    }

}
