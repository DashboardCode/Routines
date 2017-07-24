using NLog;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected
{
    static class NLogExtensions
    {
        public static void AppendRoutineTag(this LogEventInfo logEventInfo, RoutineTag routineTag)
        {
            logEventInfo.Properties[nameof(RoutineTag.CorrelationToken)] = routineTag.CorrelationToken;
            logEventInfo.Properties[nameof(RoutineTag.Namespace)] = routineTag.Namespace;
            logEventInfo.Properties[nameof(RoutineTag.Type)] = routineTag.Type;
            logEventInfo.Properties[nameof(RoutineTag.Member)] = routineTag.Member;
            logEventInfo.Properties["Title"] = $"{routineTag.Type}.{routineTag.Member}; {routineTag.Namespace}; {routineTag.CorrelationToken}";
        }
    }
}
