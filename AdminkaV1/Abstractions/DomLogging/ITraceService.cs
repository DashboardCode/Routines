using System;

namespace Vse.AdminkaV1.DomLogging
{
    public interface ITraceService
    {
        Trace GetTrace(Guid correlationToken);
    }
}
