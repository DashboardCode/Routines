using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.WcfClient.TraceServiceReference;
using DashboardCode.AdminkaV1.LoggingDom;

namespace DashboardCode.AdminkaV1.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceClient : ITraceService
    {
        public LoggingDom.Trace GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceClient();
            try
            {
                return client.GetTrace(correlationToken);
            }
            catch (FaultException<RoutineError> ex)
            {
                if (ex.Detail.UserContextExceptionCode != null)
                {
                    var baseException = new UserContextException(ex.Message, ex, ex.Detail.UserContextExceptionCode);
                    baseException.CopyData(ex.Detail.Data);
                    throw baseException;
                }
                else
                {
                    ex.CopyData(ex.Detail.Data);
                }
                throw;
            }
        }
    }
}
