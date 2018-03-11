using System;
using System.Diagnostics;

namespace DashboardCode.Routines.Injected
{
    public class VerboseMessage
    {
        public readonly DateTime DateTime;
        public readonly string Message;
        public readonly StackTrace StackTrace;

        public VerboseMessage(DateTime dateTime, string message, StackTrace stackTrace)
        {
            DateTime = dateTime;
            Message = message;
            StackTrace = stackTrace;
        }
    }
}