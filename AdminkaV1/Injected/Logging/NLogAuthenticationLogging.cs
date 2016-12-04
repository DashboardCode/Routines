using NLog;
using System;
using Vse.Routines;

namespace Vse.AdminkaV1.Injected.Logging
{
    public class NLogAuthenticationLogging : IAuthenticationLogging
    {
        private readonly Logger authenticationLogger = LogManager.GetLogger("Authentication");
        
        public void TraceAuthentication(RoutineTag routineTag, string message)
        {
            var logEventInfo = new LogEventInfo()
            {
                Message = message,
                Level = LogLevel.Info,
                TimeStamp = DateTime.Now
            };
            logEventInfo.AppendRoutineTag(routineTag);
            authenticationLogger.Log(logEventInfo);
        }
    }
}
