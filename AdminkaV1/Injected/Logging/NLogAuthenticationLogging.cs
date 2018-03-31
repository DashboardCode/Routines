using System;
using NLog;

using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class NLogAuthenticationLogging : IAuthenticationLogging
    {
        // NLOG logger is thread safe
        private readonly Logger authenticationLogger = LogManager.GetLogger("Authentication");
        
        public void TraceAuthentication(Guid correlationToken, MemberTag memberTag, string message)
        {
            var logEventInfo = new LogEventInfo()
            {
                Message = message,
                Level = LogLevel.Info,
                TimeStamp = DateTime.Now
            };
            logEventInfo.AppendRoutineTag(correlationToken, memberTag);
            authenticationLogger.Log(logEventInfo);
        }
    }
}