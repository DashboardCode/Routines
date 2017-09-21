using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.WcfClient.TraceServiceReference;
using System.Text;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceWcfClient : ITraceService
    {
        public LoggingDom.Trace GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceClient();
            try
            {
                var t = client.GetTrace(correlationToken);
                //return t;
                throw new NotImplementedException("Use types from assymbly doesn't work correctly.May be it need data contract on each type and property? But this is impossible");
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

        public static void AnalyzeWcfClientException(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is FaultException<RoutineError> faultException)
                AppendFaultException(stringBuilder, faultException);
        }
    }
}
