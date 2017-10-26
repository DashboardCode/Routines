using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    static class RoutineErrorDataContractConstants
    {
        public const string RoutineGuidNamespace         = DataContractConstants.BaseNamespace;
        public const string RoutineErrorNamespace        = DataContractConstants.BaseNamespace;
        public const string AuthenticationFaultNamespace = DataContractConstants.BaseNamespace;
        public const string FaultCodeNamespace           = DataContractConstants.BaseNamespace;
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.RoutineErrorNamespace)]
    public class RoutineError
    {
        [DataMember]
        public RoutineGuid RoutineGuid         { get; set; }
        [DataMember]
        public string Message                  { get; set; }
        [DataMember]
        public string UserContextExceptionCode { get; set; }
        [DataMember]
        public string Details                  { get; set; }
        /// <summary>
        /// Use Data to add a "exception processed", "event alarmed" flags to avoid multiple alarms 
        /// </summary>
        [DataMember]
        public Dictionary<string,string> Data  { get; set; }
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.AuthenticationFaultNamespace)]
    public class AuthenticationFault
    {
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.RoutineGuidNamespace)]
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