using System;
using System.ServiceModel;
using DashboardCode.Routines;
using TraceServiceReference;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceProxy : ITraceService
    {
        public Trace GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceClient();
            try
            {
                var task = client.GetTraceAsync(correlationToken);
                var trace = task.Result;
                return trace;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count > 0 && ex.InnerExceptions[0] is FaultException<RoutineError> faultException)
                {
                    if (faultException.Detail.AdminkaExceptionCode != null)
                    {
                        var baseException = new AdminkaException(
                            faultException.Message, faultException, faultException.Detail.AdminkaExceptionCode);
                        baseException.CopyData(faultException.Detail.Data);
                        throw baseException;
                    }
                    else
                    {
                        ex.CopyData(faultException.Detail.Data);
                    }
                }
                throw;
            }
        }
    }
}
