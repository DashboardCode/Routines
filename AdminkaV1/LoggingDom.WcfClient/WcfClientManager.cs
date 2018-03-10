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
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.MemberTag)}.{nameof(routineError.CorrelationToken)}", routineError.CorrelationToken.ToString());
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.MemberTag)}.{nameof(routineError.MemberTag.Namespace)}", routineError.MemberTag.Namespace);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.MemberTag)}.{nameof(routineError.MemberTag.Type)}", routineError.MemberTag.Type);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.MemberTag)}.{nameof(routineError.MemberTag.Member)}", routineError.MemberTag.Member);
            stringBuilder.Append("   ").AppendMarkdownProperty($"{nameof(routineError)}.{nameof(routineError.Details)}", routineError.Details);
        }

        public static void AppendWcfClientFaultException(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is FaultException<RoutineError> faultException)
                AppendFaultException(stringBuilder, faultException);
        }
    }
}