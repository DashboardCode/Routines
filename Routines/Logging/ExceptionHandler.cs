using System;
using System.Runtime.ExceptionServices;

namespace DashboardCode.Routines.Logging
{
    public class ExceptionHandler 
    {
        private readonly Action<Exception> logException;
        private readonly Func<Exception, Exception>  transformException;

        public ExceptionHandler(
            Action<Exception> logException, 
            Func<Exception, Exception>  transformException
            )
        {
            this.logException = logException;
            this.transformException = transformException;
        }

        public void Handle(Func<(Action, Action<bool>)> start)
        {
            (Action action, Action<bool> onFinish) = start();
            bool isSuccess = false;
            try
            {
                action();
                isSuccess = true;
            }
            catch (Exception exception)
            {
                logException(exception);
                var transformedException = transformException(exception);
                //  NOTE: may be there is a sensce to create alternative handler ExceptionHandler2 that will log transformException
                //  try
                //  {
                //  }
                //  catch(Exception exceptionOnExceptionTransformation)
                //  {
                //     logExceptionOnTransformation(dateTime, exceptionOnExceptionTransformation);
                //     var exceptionAsMessage = new Exception("Excepion on exception transformation", exceptionOnExceptionTransformation);
                //     logException(dateTime, exceptionAsMessage);
                //  }
                if (exception == transformedException)
                {
                    // NOTE: https://connect.microsoft.com/VisualStudio/feedback/details/689516/exceptiondispatchinfo-api-modifications
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
                    var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);

                    exceptionDispatchInfo.Throw();
                }
                throw transformedException;
            }
            finally
            {
                onFinish(isSuccess);
                //stopWatch.Stop(); 
            }
        }
    }
} 