using System;
using System.Collections.Generic;

namespace Vse.Routines.Injected
{
    public interface IBasicLogging: IActivityLogging, IVerboseLogging
    {
        bool UseBufferForVerbose { get; }
        bool VerboseWithStackTrace { get; }
        void LogVerbose(DateTime dateTime, string message);
        void LogBufferedVerbose(List<VerboseMessage> verboseMessages);
        void LogException(DateTime dateTime, Exception exception);
    }
}
