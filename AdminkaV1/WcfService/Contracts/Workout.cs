using System;
using System.Runtime.Serialization;

namespace DashboardCode.AdminkaV1.Wcf.Messaging.Contracts
{
    static class RoutineErrorDataContractConstants
    {
        public const string RoutineGuid         = "https://adminka-v1.dashboardcode.com";
        public const string RoutineError        = "https://adminka-v1.dashboardcode.com";
        public const string AuthenticationFault = "https://adminka-v1.dashboardcode.com";
        public const string FaultCode           = "https://adminka-v1.dashboardcode.com";
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.RoutineError)]
    public class RoutineError
    {
        [DataMember]
        public RoutineGuid RoutineGuid           { get; set; }
        [DataMember]
        public string Message                  { get; set; }
        [DataMember]
        public string UserContextExceptionCode { get; set; }
        [DataMember]
        public string Details                  { get; set; }
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.AuthenticationFault)]
    public class AuthenticationFault
    {
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.RoutineGuid)]
    public class RoutineGuid
    {
        [DataMember]
        public Guid CorrelationToken { get; set; }
        [DataMember]
        public string Namespace      { get; set; }
        [DataMember]
        public string Type           { get; set; }
        [DataMember]
        public string Member         { get; set; }
    }
}