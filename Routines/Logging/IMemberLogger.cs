using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Logging
{
    public interface IDataLogger
    {
        void Input(DateTime dateTime, object input);
        void Output(DateTime dateTime, object output);
    }

    public interface IActivityLogger
    {
        void LogActivityStart(DateTime dateTime);
        void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess);
    }

    public interface IVerboseLogger
    {
        void LogVerbose(DateTime dateTime, string message);
    }

    public interface IBufferedVerboseLogger
    {
        void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages);
    }

    public interface IExceptionLogger
    {
        void LogException(DateTime dateTime, Exception exception);
    }

    public interface IErrorLogger
    {
        void LogError(DateTime dateTime, string message);
    }

    public interface IMemberLogger: IActivityLogger, IVerboseLogger, IBufferedVerboseLogger, IDataLogger, IExceptionLogger, IErrorLogger
    {
    }
}