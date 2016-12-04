using System;

namespace Vse.Routines.Injected
{
    public class ExceptionAdapter : IExceptionAdapter
    {
        private readonly Action<DateTime, Exception> logException;
        private readonly Func<Exception, Exception> transformException;
        public ExceptionAdapter(Action<DateTime, Exception> logException, Func<Exception, Exception> transformException)
        {
            this.logException = logException;
            this.transformException = transformException;
        }
        public void LogException(DateTime dateTime, Exception exception)
        {
            logException(dateTime, exception);
        }
        public Exception TransformException(Exception exception)
        {
            return transformException(exception);
        }
    }
}