using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Text;
using DashboardCode.AdminkaV1.WcfClient.TraceServiceReference;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.WcfClient
{
    public static class ExceptionExtensions
    {
        internal static void CopyData(this Exception exception, Dictionary<string, string> data)
        {
            if (data != null)
                foreach (var pair in data)
                    exception.Data[pair.Key] = pair.Value;
        }

        internal static void AppendFaultException(this StringBuilder stringBuilder, FaultException<RoutineError> exception)
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

        public static void AnalyzeWcfClientException(this StringBuilder stringBuilder, Exception exception)
        {
            if (exception is FaultException<RoutineError> faultException)
                stringBuilder.AppendFaultException(faultException);
        }
    }
}
