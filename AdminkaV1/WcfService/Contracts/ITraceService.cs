using System;
using System.ServiceModel;
using Vse.AdminkaV1.DomLogging;

namespace Vse.AdminkaV1.WcfService.Contracts
{
    public static class TraceServiceContractConstants
    {
        public const string PortName = nameof(TraceService);
        public const string ServiceContract = "https://github.com/rpokrovskij/Vse/Adminka-v1";
    }
    [ServiceContract(Namespace = TraceServiceContractConstants.ServiceContract, Name = TraceServiceContractConstants.PortName)]
    interface ITraceService
    {
        [OperationContract]
        [FaultContract(typeof(RoutineError)), FaultContract(typeof(AuthenticationFault))]
        Trace GetTrace(Guid correlationToken);
    }
}
