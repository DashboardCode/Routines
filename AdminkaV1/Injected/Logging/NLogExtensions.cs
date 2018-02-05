using NLog;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    static class NLogExtensions
    {
        public static void AppendRoutineTag(this LogEventInfo logEventInfo, RoutineGuid routineGuid)
        {
            logEventInfo.Properties[nameof(RoutineGuid.CorrelationToken)] = routineGuid.CorrelationToken;
            logEventInfo.Properties[nameof(MemberTag.Namespace)] = routineGuid.MemberTag.Namespace;
            logEventInfo.Properties[nameof(MemberTag.Type)] = routineGuid.MemberTag.Type;
            logEventInfo.Properties[nameof(MemberTag.Member)] = routineGuid.MemberTag.Member;
            logEventInfo.Properties["Title"] = $"{routineGuid.MemberTag.Type}.{routineGuid.MemberTag.Member}; {routineGuid.MemberTag.Namespace}; {routineGuid.CorrelationToken}";
        }
    }
}