using System;

namespace DashboardCode.Routines.Injected
{
    public interface IExceptionAdapter
    {
        void LogException(DateTime dateTime, Exception exception);
        Exception TransformException(Exception exception);
    }
}
