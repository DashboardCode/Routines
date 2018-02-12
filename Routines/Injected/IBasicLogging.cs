using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public interface IBasicLogging: IActivityLogger, IDataLogger
    {
        void LogVerbose(DateTime dateTime, string message);
        void LogBufferedVerbose(List<VerboseMessage> verboseMessages);
        void LogException(DateTime dateTime, Exception exception);
    }
}