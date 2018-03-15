using System;
using System.Threading.Tasks;
using System.ServiceModel;

using TraceServiceReference;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceAsyncProxy : ITraceServiceAsync
    {
        public async Task<Trace> GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceClient();
            try
            {
                return await client.GetTraceAsync(correlationToken);
            }
            catch (FaultException<RoutineError> ex)
            {
                if (ex.Detail.AdminkaExceptionCode != null)
                {
                    var baseException = new AdminkaException(ex.Message, ex, ex.Detail.AdminkaExceptionCode);
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