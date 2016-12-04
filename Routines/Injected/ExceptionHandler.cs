using System;
using System.Diagnostics;

namespace Vse.Routines.Injected
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionAdapter defaultExceptionAdapter;
        private readonly Action<long> monitorRoutineDurationTicks;
        public ExceptionHandler(
            IExceptionAdapter defaultExceptionAdapter,
            Action<long> monitorRoutineDurationTicks
            )
        {
            this.defaultExceptionAdapter = defaultExceptionAdapter;
            this.monitorRoutineDurationTicks = monitorRoutineDurationTicks;
        }

        public void Handle(Action action, Action onFailure)
        {
            var stopWatch = Stopwatch.StartNew();
            try
            {
                action();
            }
            catch (Exception exception)
            {
                var dateTime = DateTime.Now;
                defaultExceptionAdapter.LogException(dateTime, exception);
                var transformedException = exception;
                try
                {
                    transformedException = defaultExceptionAdapter.TransformException(exception);
                }
                catch(Exception exceptionOnExceptionTransformation)
                {
                    var message = new Exception("Excepion during exception transformation", exceptionOnExceptionTransformation);
                    defaultExceptionAdapter.LogException(dateTime, message);
                }

                onFailure();

                if (exception == transformedException)
                    throw;
                throw transformedException;
            }
            finally
            {
                stopWatch.Stop();
                monitorRoutineDurationTicks?.Invoke(stopWatch.ElapsedTicks);
            }
        }
    }
}
