using System;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;

namespace DashboardCode.Routines
{

    public interface IExceptionHandler
    {
        void Handle(Action action, Action<bool> onFinish);
        Task HandleAsync(Func<Task> action, Action<bool> onFinish);
    }

    public class ExceptionHandler : IExceptionHandler
    {
        private readonly Action<Exception> handleException;
        private readonly Func<Exception, Exception>  transformException;

        public ExceptionHandler(
            Action<Exception> handleException, 
            Func<Exception, Exception>  transformException
            )
        {
            this.handleException = handleException;
            this.transformException = transformException;
        }

        public void Handle(Action action, Action<bool> onFinish)
        {
            bool isSuccess = false;
            try
            {
                action();
                isSuccess = true;
            }
            catch (Exception exception)
            {
                handleException(exception);
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

        public async Task HandleAsync(Func<Task> action, Action<bool> onFinish)
        {
            bool isSuccess = false;
            try
            {
                await action();
                isSuccess = true;
            }
            catch (Exception exception)
            {
                handleException(exception);
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

    public class SimpleExceptionHandler : IExceptionHandler
    {
        private readonly Action<Exception> handleException;

        public SimpleExceptionHandler(
            Action<Exception> handleException
            )
        {
            this.handleException = handleException;
        }

        public void Handle(Action action, Action<bool> onFinish)
        {
            bool isSuccess = false;
            try
            {
                action();
                isSuccess = true;
            }
            catch (Exception exception)
            {
                handleException(exception);
            }
            finally
            {
                onFinish(isSuccess);
            }
        }

        public async Task HandleAsync(Func<Task> action, Action<bool> onFinish)
        {
            bool isSuccess = false;
            try
            {
                await action();
                isSuccess = true;
            }
            catch (Exception exception)
            {
                handleException(exception);
            }
            finally
            {
                onFinish(isSuccess);
            }
        }
    }
}