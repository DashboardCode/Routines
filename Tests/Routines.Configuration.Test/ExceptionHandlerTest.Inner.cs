using System;
using System.IO;
using System.Collections.Generic;

using DashboardCode.Routines.Injected;

namespace DashboardCode.Routines.Configuration.Test
{
    partial class ExceptionHandlerTest
    {
        private static void TestMethod()
        {
            var list = new List<string>();
            var exceptionAdapter = new ExceptionAdapter(list);
            var exceptionHandler = new ExceptionHandler(
                ex=>exceptionAdapter.LogException(DateTime.Now, ex), 
                exceptionAdapter.TransformException);
            try
            {
                exceptionHandler.Handle(
                    () => {
                        var source = File.OpenText("notexisted");
                        Action onSuccess = () => { };
                        Action<bool> onFinally = (isSuccess) => { };
                        return (onSuccess, onFinally);
                    });
            }
            catch (Exception ex)
            {
                var st = ex.StackTrace;
                if (!st.Contains("ExceptionHandlerTest.Inner.cs:line 19"))
                {
                    throw;
                }
            }
        }

        class ExceptionAdapter 
        {
            readonly List<string> list;
            public ExceptionAdapter(List<string> list)
            {
                this.list = list;
            }

            public void LogException(DateTime dateTime, Exception exception)
            {
                list.Add(ExceptionExtensions.Markdown(exception));
            }

            public Exception TransformException(Exception exception)
            {
                return exception;
            }
        }
    }
}