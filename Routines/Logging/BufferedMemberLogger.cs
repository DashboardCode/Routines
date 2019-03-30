using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Logging
{
    class BufferedMemberLogger : IMemberLogger
    {
        readonly IMemberLogger memberLogger;
        readonly Action<DateTime, string> logVerbose;
        readonly bool startActivity;

        public BufferedMemberLogger(IMemberLogger memberLogger, Action<DateTime, string> logVerbose, bool startActivity)
        {
            this.memberLogger = memberLogger;
            this.logVerbose = logVerbose;
            this.startActivity = startActivity;
        }

        public void Input(DateTime dateTime, object input)
        {
            memberLogger.Input(dateTime, input);
        }

        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            memberLogger.LogActivityFinish(dateTime, timeSpan, isSuccess);
        }

        public void LogActivityStart(DateTime dateTime)
        {
            if (startActivity)
                memberLogger.LogActivityStart(dateTime);
        }

        public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
        {
            memberLogger.LogBufferedVerbose(verboseMessages);
        }

        public void LogError(DateTime dateTime, string message)
        {
            memberLogger.LogError(dateTime, message);
        }

        public void LogException(DateTime dateTime, Exception exception)
        {
            memberLogger.LogException(dateTime, exception);
        }

        public void LogVerbose(DateTime dateTime, string message)
        {
            logVerbose(dateTime, message);
        }

        public void Output(DateTime dateTime, object output)
        {
            memberLogger.Output(dateTime, output);
        }
    }
}
