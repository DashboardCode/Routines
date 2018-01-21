using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DashboardCode.Routines;
using TraceServiceReference;


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

        //internal static async void LogExceptionAsync(AggregateException ex)
        //{
        //    if (ex.InnerExceptions.Count > 0 && ex.InnerExceptions[0] is FaultException<RoutineError> faultException)
        //    {
        //        if (faultException.Detail.AdminkaExceptionCode != null)
        //        {
        //            var baseException = new UserContextException(ex.Message, ex, faultException.Detail.AdminkaExceptionCode);
        //            baseException.CopyData(faultException.Detail.Data);
        //            throw baseException;
        //        }
        //        else
        //        {
        //            ex.CopyData(faultException.Detail.Data);
        //        }
        //    }
        //    throw;
        //}

        public static void AppendWcfClientFaultException(StringBuilder stringBuilder, Exception exception)
        {
            TraceServiceProxy.AppendWcfClientFaultException(stringBuilder, exception);
        }

    }
}
