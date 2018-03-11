using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public interface IDataLogger
    {
        void Input(DateTime dt, object input);
        void Output(DateTime dt, object output);
    }

    public interface IVerboseLogger
    {
        void LogVerbose(DateTime dateTime, string message);
    }

    public interface IBufferedVerboseLogger
    {
        void LogBufferedVerbose(List<VerboseMessage> verboseMessages);
    }

    public interface IExceptionLogger
    {
        void LogException(DateTime dateTime, Exception exception);
    }

    public interface IActivityLogger
    {
        void LogActivityStart(DateTime dateTime);
        void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess);
    }

    public interface IRoutineLogger: IActivityLogger, IVerboseLogger, IBufferedVerboseLogger, IDataLogger, IExceptionLogger
    {
    }
}