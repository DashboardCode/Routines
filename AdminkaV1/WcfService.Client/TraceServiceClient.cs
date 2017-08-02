using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.DomLogging;
using DashboardCode.AdminkaV1.Wcf.Messaging.Client.TraceServiceReference;

namespace DashboardCode.AdminkaV1.Wcf.Messaging.Client
{
    // TODO: this code can be generated with T4
    public class TraceServiceClient : ITraceService
    {
        public DomLogging.Trace GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceReference.TraceServiceClient();
            try
            {
                return client.GetTrace(correlationToken);
            }
            catch (FaultException<RoutineError> ex)
            {
                var baseException = (Exception)ex;
                if (ex.Detail.UserContextExceptionCode != null)
                    baseException = new UserContextException(ex.Message, ex, ex.Detail.UserContextExceptionCode);
                baseException.Data["RemoteUserContextExceptionCode"] = ex.Detail.UserContextExceptionCode;
                baseException.Data["RemoteCorrelationToken"] = ex.Detail.RoutineGuid.CorrelationToken;
                baseException.Data["RemoteNamespace"] = ex.Detail.RoutineGuid.Namespace;
                baseException.Data["RemoteType"] = ex.Detail.RoutineGuid.Type;
                baseException.Data["RemoteMember"] = ex.Detail.RoutineGuid.Member;
                baseException.Data["RemoteDetails"] = ex.Detail.Details;
                if (baseException != ex)
                    throw baseException;
                throw;
            }
        }
    }
}
