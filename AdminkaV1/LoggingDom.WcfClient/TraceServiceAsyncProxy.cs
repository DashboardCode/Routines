using System;
using System.Threading.Tasks;
using System.ServiceModel;
using DashboardCode.Routines;

using TraceServiceReference;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceAsyncProxy : ITraceServiceAsync
    {
        private readonly Func<TraceServiceClient> clientFactory;
        private readonly Action<string, object> verbose;

        public TraceServiceAsyncProxy(TraceServiceConfiguration traceServiceConfiguration, Action<string, object> verbose)
        {
            var endpointConfiguration = TraceServiceClient.EndpointConfiguration.BasicHttpBinding_TraceService;
            clientFactory = () => new TraceServiceClient(endpointConfiguration, traceServiceConfiguration.RemoteAddress);
            this.verbose = verbose;
        }

        public async Task<List<VerboseRecord>> GetTrace(Guid correlationToken)
        {
            using var client = clientFactory();
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