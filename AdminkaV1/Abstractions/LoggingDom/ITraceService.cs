using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.LoggingDom
{
    public interface ITraceService
    {
        List<VerboseRecord> GetTrace(Guid correlationToken);
    }
}