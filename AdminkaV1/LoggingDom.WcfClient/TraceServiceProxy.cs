using System;
using System.Collections.Generic;
using System.ServiceModel;

using DashboardCode.Routines;
using TraceServiceReference;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceProxy : ITraceService
    {
        public List<VerboseRecord> GetTrace(Guid correlationToken)
        {
            using var client = new TraceServiceClient();
            try
            {
                //#pragma warning disable AsyncFixer04 // A disposable object used in a fire & forget async call
                var r = client.GetTraceAsync(correlationToken).Result;
                //#pragma warning restore AsyncFixer04 // A disposable object used in a fire & forget async call
                //var trace = task.Result;
                return r;
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