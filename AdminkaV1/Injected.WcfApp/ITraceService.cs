using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.LoggingDom;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    public static class TraceServiceContractConstants
    {
        public const string PortName = "TraceService";
        public const string ServiceContract = DataContractConstants.BaseNamespace + "/" + PortName;
    }

    // TODO: generate ITraceService and TraceService with T4 on compile time or even with Roslyn on runtime at startup
    [ServiceContract(Namespace = TraceServiceContractConstants.ServiceContract, 
                     Name = TraceServiceContractConstants.PortName)]
    interface ITraceService
    {
        [OperationContract]
        [FaultContract(typeof(RoutineError)), FaultContract(typeof(AuthenticationFault))]
        Trace GetTrace(Guid correlationToken);
    }
}
