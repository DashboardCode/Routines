using System;
using System.Text;
using System.ServiceModel;
using TraceServiceReference;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    public static class WcfClientManager
    {
        internal static void AppendFaultException(StringBuilder stringBuilder, FaultException<RoutineError> exception)
        {
            var routineError = exception.Detail;
            stringBuilder.AppendMarkdownLine(nameof(FaultException<RoutineError>) + " specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(exception)}.{nameof(exception.Code)}.{nameof(exception.Code.Name)}", exception.Code.Name);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.AdminkaExceptionCode)}", routineError.AdminkaExceptionCode);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.RoutineGuid)}.{nameof(routineError.RoutineGuid.CorrelationToken)}", routineError.RoutineGuid.CorrelationToken.ToString());
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.RoutineGuid)}.{nameof(routineError.RoutineGuid.Namespace)}", routineError.RoutineGuid.Namespace);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.RoutineGuid)}.{nameof(routineError.RoutineGuid.Type)}", routineError.RoutineGuid.Type);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.RoutineGuid)}.{nameof(routineError.RoutineGuid.Member)}", routineError.RoutineGuid.Member);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.Details)}", routineError.Details);
        }

        public static void AppendWcfClientFaultException(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is FaultException<RoutineError> faultException)
                AppendFaultException(stringBuilder, faultException);
        }
    }
}