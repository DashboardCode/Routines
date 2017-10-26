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

        //internal static async void LogExceptionAsync(AggregateException ex)
        //{
        //    if (ex.InnerExceptions.Count > 0 && ex.InnerExceptions[0] is FaultException<RoutineError> faultException)
        //    {
        //        if (faultException.Detail.UserContextExceptionCode != null)
        //        {
        //            var baseException = new UserContextException(ex.Message, ex, faultException.Detail.UserContextExceptionCode);
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

        internal static void AppendFaultException(StringBuilder stringBuilder, FaultException<RoutineError> exception)
        {
            var routineError = exception.Detail;
            stringBuilder.AppendMarkdownLine(nameof(FaultException<RoutineError>) + " specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("FaultCode.Name", exception.Code.Name);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.UserContextExceptionCode", routineError.UserContextExceptionCode);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.CorrelationToken", routineError.RoutineGuid.CorrelationToken.ToString());
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.Namespace", routineError.RoutineGuid.Namespace);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.Type", routineError.RoutineGuid.Type);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.Member", routineError.RoutineGuid.Member);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.Details", routineError.Details);
        }

        public static void AppendWcfClientFaultException(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is FaultException<RoutineError> faultException)
                AppendFaultException(stringBuilder, faultException);
        }
    }
}
