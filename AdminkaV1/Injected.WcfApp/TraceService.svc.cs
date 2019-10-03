using System;
using System.Linq;
using System.Collections.Generic;

using DashboardCode.AdminkaV1.LoggingDom;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    public class TraceService : ITraceService
    {
        public List<VerboseRecord> GetTrace(Guid searchForCorrelationToken)
        {
            var routine = new WcfRoutine(new DashboardCode.Routines.MemberTag(this), RoutineErrorDataContractConstants.FaultCodeNamespace, new { searchForCorrelationToken });
            return routine.Handle((container, closure) => container.ResolveLoggingDomDbContextHandler().HandleRepository<List< VerboseRecord >, VerboseRecord>( rep =>
            {
                return rep.Query().Where(e=>e.CorrelationToken== searchForCorrelationToken).ToList();
            }));
        }
    }
}