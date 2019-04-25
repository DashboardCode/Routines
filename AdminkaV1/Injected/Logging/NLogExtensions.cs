using System;
using NLog;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    static class NLogExtensions
    {
        public static void AppendRoutineTag(this LogEventInfo logEventInfo, DateTime dateTime, Guid correlationToken, MemberTag memberTag)
        {
            logEventInfo.Properties["CorrelationToken"] = correlationToken;
            logEventInfo.Properties["Time"] = dateTime.ToString("s");
            logEventInfo.Properties[nameof(MemberTag.Namespace)] = memberTag.Namespace;
            logEventInfo.Properties[nameof(MemberTag.Type)] = memberTag.Type;
            logEventInfo.Properties[nameof(MemberTag.Member)] = memberTag.Member;
            logEventInfo.Properties["Title"] = $"{memberTag.Type}.{memberTag.Member}; {memberTag.Namespace}; {correlationToken}";
        }
    }
}