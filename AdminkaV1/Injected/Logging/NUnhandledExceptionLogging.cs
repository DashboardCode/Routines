using System;
using NLog;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class NUnhandledExceptionLogging : IUnhandledExceptionLogging
    {
        private readonly Logger errorLogger = LogManager.GetLogger("Unhandled");

        public void TraceError(Guid correlationToken, string message)
        {
            var dateTime = DateTime.Now;
            var logEventInfo = new LogEventInfo()
            {
                Message = message,
                Level = LogLevel.Info,
                TimeStamp = dateTime
            };
            logEventInfo.Properties["CorrelationToken"] = correlationToken;
            logEventInfo.Properties["Time"] = dateTime.ToString("s");
            errorLogger.Log(logEventInfo);
        }
    }
}
