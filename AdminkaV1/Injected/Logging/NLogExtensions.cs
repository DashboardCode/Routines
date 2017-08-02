using NLog;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected
{
    static class NLogExtensions
    {
        public static void AppendRoutineTag(this LogEventInfo logEventInfo, MemberGuid routineTag)
        {
            logEventInfo.Properties[nameof(MemberGuid.CorrelationToken)] = routineTag.CorrelationToken;
            logEventInfo.Properties[nameof(MemberGuid.Namespace)] = routineTag.Namespace;
            logEventInfo.Properties[nameof(MemberGuid.Type)] = routineTag.Type;
            logEventInfo.Properties[nameof(MemberGuid.Member)] = routineTag.Member;
            logEventInfo.Properties["Title"] = $"{routineTag.Type}.{routineTag.Member}; {routineTag.Namespace}; {routineTag.CorrelationToken}";
        }
    }
}
