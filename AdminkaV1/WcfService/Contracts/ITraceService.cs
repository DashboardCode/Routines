using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.DomLogging;

namespace DashboardCode.AdminkaV1.WcfService.Contracts
{
    public static class TraceServiceContractConstants
    {
        public const string PortName = nameof(TraceService);
        public const string ServiceContract = "https://DashboardCode.com/Adminka-V1";
    }
    [ServiceContract(Namespace = TraceServiceContractConstants.ServiceContract, Name = TraceServiceContractConstants.PortName)]
    interface ITraceService
    {
        [OperationContract]
        [FaultContract(typeof(RoutineError)), FaultContract(typeof(AuthenticationFault))]
        Trace GetTrace(Guid correlationToken);
    }
}
