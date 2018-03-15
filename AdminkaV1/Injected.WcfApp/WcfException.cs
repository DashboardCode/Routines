using System;
using System.ServiceModel;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    [Serializable]
    public class WcfException : FaultException<RoutineError>
    {
        //public WcfException(
        //    RoutineError routineError,
        //    FaultReason reason,
        //    FaultCode code)
        //    : base(routineError, reason, code)
        //{
        //}

        public WcfException(
            RoutineError routineError,
            string message,
            string code,
            string faultCodeNamespace)
            : base(routineError, new FaultReason(message), new FaultCode(code, faultCodeNamespace))
        {
        }
    }
}