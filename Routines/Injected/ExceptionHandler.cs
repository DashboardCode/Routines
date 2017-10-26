using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace DashboardCode.Routines.Injected
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionAdapter exceptionAdapter;
        private readonly Action<long> monitorDurationTicks;
        public ExceptionHandler(
            IExceptionAdapter exceptionAdapter,
            Action<long> monitorDurationTicks
            )
        {
            this.exceptionAdapter = exceptionAdapter;
            this.monitorDurationTicks = monitorDurationTicks;
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
                exceptionAdapter.LogException(dateTime, exception);
                var transformedException = exception;
                try
                {
                    transformedException = exceptionAdapter.TransformException(exception);
                }
                catch(Exception exceptionOnExceptionTransformation)
                {
                    var message = new Exception("Excepion on exception transformation", exceptionOnExceptionTransformation);
                    exceptionAdapter.LogException(dateTime, message);
                }

                onFailure();

                if (exception == transformedException)
                {
                    // Preserve stack trace: after this catched exception's StackTrace will contains two logical parts
                    // Compare those two ways two rethrow of the exception (actual error in 'ExceptionHandlerTest.Inner.cs:line 18')

                    // 1) throw; then StackTrace text looks like:
                    // at System.IO.File.OpenText(String path)
                    // at DashboardCode.Routines.Configuration.Test.ExceptionHandlerTest.<> c.< TestMethod > b__1_0() in D:\..\ExceptionHandlerTest.Inner.cs:line 18
                    // at DashboardCode.Routines.Injected.ExceptionHandler.Handle(Action action, Action onFailure) in D:\..\ExceptionHandler.cs:line 46
                    // at DashboardCode.Routines.Configuration.Test.ExceptionHandlerTest.TestMethod() in D:\..\ExceptionHandlerTest.Inner.cs:line 16

                    // 2) ExceptionDispatchInfo.Capture(exception).Throw(); then StackTrace text looks like:
                    // at System.IO.File.OpenText(String path)
                    // at DashboardCode.Routines.Configuration.Test.ExceptionHandlerTest.<> c.< TestMethod > b__1_0() in D:\..\ExceptionHandlerTest.Inner.cs:line 18
                    // at DashboardCode.Routines.Injected.ExceptionHandler.Handle(Action action, Action onFailure) in D:\..\ExceptionHandler.cs:line 25
                    // --- End of stack trace from previous location where exception was thrown ---
                    // at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
                    // at DashboardCode.Routines.Injected.ExceptionHandler.Handle(Action action, Action onFailure) in D:\..\ExceptionHandler.cs:line 50
                    // at DashboardCode.Routines.Configuration.Test.ExceptionHandlerTest.TestMethod() in D:\..\ExceptionHandlerTest.Inner.cs:line 16
                    // NOTE: https://connect.microsoft.com/VisualStudio/feedback/details/689516/exceptiondispatchinfo-api-modifications
                    var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                    exceptionDispatchInfo.Throw();
                }
                throw transformedException;
            }
            finally
            {
                stopWatch.Stop();
                monitorDurationTicks?.Invoke(stopWatch.ElapsedTicks);
            }
        }
    }
}
