using System;
using DashboardCode.AdminkaV1.DomLogging;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines;
using DataContactConstants = DashboardCode.AdminkaV1.Wcf.Messaging.Contracts.RoutineErrorDataContractConstants;
using IService = DashboardCode.AdminkaV1.Wcf.Messaging.Contracts.ITraceService;

namespace DashboardCode.AdminkaV1.Wcf.Messaging
{
    public class TraceService : IService
    {
        public Trace GetTrace(Guid searchForCorrelationToken)
        {
            var input = new { searchForCorrelationToken = searchForCorrelationToken };
            var routine = new WcfRoutine(new MemberTag(this), DataContactConstants.FaultCode, input);
            return routine.Handle((container, dataAccess) =>
            {
                var servicesContainer = new ServicesContainer(dataAccess);
                var service = servicesContainer.ResolveTraceService();
                return service.GetTrace(searchForCorrelationToken);
            });
        }
    }
}