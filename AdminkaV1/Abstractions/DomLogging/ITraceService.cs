using System;

namespace DashboardCode.AdminkaV1.DomLogging
{
    public interface ITraceService
    {
        Trace GetTrace(Guid correlationToken);
    }
}
