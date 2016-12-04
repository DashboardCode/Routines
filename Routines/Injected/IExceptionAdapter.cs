using System;

namespace Vse.Routines.Injected
{
    public interface IExceptionAdapter
    {
        void LogException(DateTime dateTime, Exception exception);
        Exception TransformException(Exception exception);
    }
}
