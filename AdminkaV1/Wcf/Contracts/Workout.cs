using System;
using System.Runtime.Serialization;

namespace Vse.AdminkaV1.WcfService.Contracts
{
    static class RoutineErrorDataContractConstants
    {
        public const string Routine = "https://github.com/rpokrovskij/Vse/Adminka-v1";
        public const string RoutineError = "https://github.com/rpokrovskij/Vse/Adminka-v1";
        public const string AuthenticationFault = "https://github.com/rpokrovskij/Vse/Adminka-v1";
        public const string FaultCode = "https://github.com/rpokrovskij/Vse/Adminka-v1";
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.RoutineError)]
    public class RoutineError
    {
        [DataMember]
        public RoutineTag RoutineTag { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string UserContextExceptionCode { get; set; }
        [DataMember]
        public string Details { get; set; }
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.AuthenticationFault)]
    public class AuthenticationFault
    {
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract(Namespace = RoutineErrorDataContractConstants.Routine)]
    public class RoutineTag
    {
        [DataMember]
        public Guid CorrelationToken { get; set; }
        [DataMember]
        public string Namespace { get; set; }
        [DataMember]
        public string Class { get; set; }
        [DataMember]
        public string Member { get; set; }
    }
}