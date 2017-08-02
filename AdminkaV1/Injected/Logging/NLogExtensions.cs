using NLog;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected
{
    static class NLogExtensions
    {
        public static void AppendRoutineTag(this LogEventInfo logEventInfo, RoutineGuid routineTag)
        {
            logEventInfo.Properties[nameof(RoutineGuid.CorrelationToken)] = routineTag.CorrelationToken;
            logEventInfo.Properties[nameof(RoutineGuid.Namespace)] = routineTag.Namespace;
            logEventInfo.Properties[nameof(RoutineGuid.Type)] = routineTag.Type;
            logEventInfo.Properties[nameof(RoutineGuid.Member)] = routineTag.Member;
            logEventInfo.Properties["Title"] = $"{routineTag.Type}.{routineTag.Member}; {routineTag.Namespace}; {routineTag.CorrelationToken}";
        }
    }
}
