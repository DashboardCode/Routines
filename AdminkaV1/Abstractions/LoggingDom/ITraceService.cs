using System;

namespace DashboardCode.AdminkaV1.LoggingDom
{
    public interface ITraceService
    {
        Trace GetTrace(Guid correlationToken);
    }
}
