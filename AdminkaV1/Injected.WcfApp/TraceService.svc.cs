using System;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    public class TraceService : ITraceService
    {
        public Trace GetTrace(Guid searchForCorrelationToken)
        {
            var input = new { searchForCorrelationToken = searchForCorrelationToken };
            var routine = new WcfRoutine(new MemberTag(this), RoutineErrorDataContractConstants.FaultCodeNamespace, input);
            return routine.HandleServicesContainer(servicesContainer =>
            {
                var service = servicesContainer.ResolveTraceService();
                return service.GetTrace(searchForCorrelationToken);
            });
        }
    }
}