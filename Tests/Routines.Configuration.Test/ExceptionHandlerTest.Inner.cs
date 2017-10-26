﻿using DashboardCode.Routines.Injected;
using System;
using System.Collections.Generic;
using System.IO;

namespace DashboardCode.Routines.Configuration.Test
{
    partial class ExceptionHandlerTest
    {
        public static void TestMethod()
        {
            var list = new List<string>();
            var exceptionHandler = new ExceptionHandler(new ExceptionAdapter(list), null);
            try
            {
                exceptionHandler.Handle(
                    () => {
                        var source = File.OpenText("notexisted");
                    }
                    , () => {

                    }
                    );
            }
            catch (Exception ex)
            {
                var st = ex.StackTrace;
                if (!st.Contains("ExceptionHandlerTest.Inner.cs:line 18"))
                {
                    throw;
                }
            }
        }

        class ExceptionAdapter : IExceptionAdapter
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
