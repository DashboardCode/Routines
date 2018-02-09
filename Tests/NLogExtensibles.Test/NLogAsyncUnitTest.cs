using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NLog;
using NLog.Common;

namespace DashboardCode.NLogExtensibles.Test
{
    // TODO: do it more realistic N users authenticate and trace activities, verbose episodically, 
    // when 1 user verbose constantly.
    [TestClass]
    public class NLogAsyncUnitTest
    {
        const int usersCount = 1;
        const int operations = 20;
        const int puaseBetweenOperationsMs = 0;

        const int repeats = 1;

        Logger logger1;
        Logger logger2;
        NLog.Targets.Target verboseTarget;
        public NLogAsyncUnitTest()
        {
            logger1 = NLog.LogManager.GetLogger("Routine1:Tests");
            logger2 = NLog.LogManager.GetLogger("Routine2:Tests");
            verboseTarget = NLog.LogManager.Configuration.FindTargetByName("verbose3");

        }

        [TestMethod]
        public void NlogBufferingWrapperCore()
        {
            for (var r = 0; r <= repeats; r++)
            {
                var tasks = new List<Task>();
                for (var u = 0; u <= usersCount; u++)
                {
                    var task = Task.Run(
                        () =>
                        {
                            for (var o = 0; o <= operations; o++)
                            {
                                var verboseMessages = LogTestManager.Generate();
                                var count = verboseMessages.Count();
                                var i = 0;
                                var at = DateTime.Now;
                                foreach (var verbose in verboseMessages)
                                {
                                    var logEventInfo = new LogEventInfo()
                                    {
                                        Level = LogLevel.Info,
                                        TimeStamp = verbose.DateTime,
                                        Message = verbose.Message,
                                    };
                                    logEventInfo.Properties["LoggedAt"] = verbose.DateTime;
                                    logEventInfo.Properties["FlushedAt"] = at;
                                    logEventInfo.Properties["A1"] = $"testtesttest";
                                    logEventInfo.Properties["A2"] = $"testtesttest";
                                    logEventInfo.Properties["A3"] = $"testtesttest";
                                    logEventInfo.Properties["A4"] = $"testtesttest";
                                    logEventInfo.Properties["Description"] = $"BufferedVerbose";
                                    logEventInfo.Properties["Buffered"] = $"{i++}/{count}";
                                    logger1.Log(logEventInfo);
                                }
                                //LogManager.Flush();
                                Thread.Sleep(puaseBetweenOperationsMs);
                            }
                        }
                        );
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            }
        }

        [TestMethod]
        public void NlogAsyncWrapperCore()
        {
            for (var r = 0; r <= repeats; r++)
            {
                var tasks = new List<Task>();
                for (var u = 0; u <= usersCount; u++)
                {
                    var task = Task.Run(
                        () =>
                        {
                            for (var o = 0; o <= operations; o++)
                            {
                                var verboseMessages = LogTestManager.Generate();
                                var count = verboseMessages.Count();
                                var i = 0;
                                var at = DateTime.Now;
                                foreach (var verbose in verboseMessages)
                                {

                                    var logEventInfo = new LogEventInfo()
                                    {
                                        Level = LogLevel.Info,
                                        TimeStamp = verbose.DateTime,
                                        Message = verbose.Message,
                                    };
                                    logEventInfo.Properties["LoggedAt"] = verbose.DateTime;
                                    logEventInfo.Properties["FlushedAt"] = at;
                                    logEventInfo.Properties["A1"] = $"testtesttest";
                                    logEventInfo.Properties["A2"] = $"testtesttest";
                                    logEventInfo.Properties["A3"] = $"testtesttest";
                                    logEventInfo.Properties["A4"] = $"testtesttest";
                                    logEventInfo.Properties["Description"] = $"BufferedVerbose";
                                    logEventInfo.Properties["Buffered"] = $"{i++}/{count}";
                                    logger2.Log(logEventInfo);
                                }
                                Thread.Sleep(puaseBetweenOperationsMs);
                            }
                        }
                        );
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            }
        }

        [TestMethod]
        public void NlogWriteAsyncLogEventsCore()
        {
            for (var r = 0; r <= repeats; r++)
            {
                var tasks = new List<Task>();
                for (var u = 0; u <= usersCount; u++)
                {
                    var task = Task.Run(
                        () =>
                        {
                            for (var o = 0; o <= operations; o++)
                            {
                                var verboseMessages = LogTestManager.Generate();
                                var count = verboseMessages.Count();
                                var i = 0;
                                var list = new List<AsyncLogEventInfo>();
                                int originalThreadId = Thread.CurrentThread.ManagedThreadId;
                                var at = DateTime.Now;
                                foreach (var verbose in verboseMessages)
                                {

                                    var logEventInfo = new LogEventInfo()
                                    {
                                        Level = LogLevel.Info,
                                        TimeStamp = verbose.DateTime,
                                        Message = verbose.Message,
                                    };
                                    logEventInfo.Properties["LoggedAt"] = verbose.DateTime;
                                    logEventInfo.Properties["FlushedAt"] = at;
                                    logEventInfo.Properties["A1"] = $"testtesttest";
                                    logEventInfo.Properties["A2"] = $"testtesttest";
                                    logEventInfo.Properties["A3"] = $"testtesttest";
                                    logEventInfo.Properties["A4"] = $"testtesttest";
                                    logEventInfo.Properties["Description"] = $"BufferedVerbose";
                                    logEventInfo.Properties["Buffered"] = $"{i++}/{count}";

                                    var asyncLogEventInfo = logEventInfo.WithContinuation((ex) =>
                                    {
                                        if (ex != null)
                                        {
                                            if (NLog.LogManager.ThrowExceptions && Thread.CurrentThread.ManagedThreadId == originalThreadId)
                                            {
                                                throw new NLogRuntimeException("Exception occurred in NLog", ex);
                                            }
                                        }
                                    });
                                    list.Add(asyncLogEventInfo);

                                }
                                verboseTarget.WriteAsyncLogEvents(list.ToArray());
                                Thread.Sleep(puaseBetweenOperationsMs);
                            }
                        }
                        );
                    tasks.Add(task);

                }
                Task.WaitAll(tasks.ToArray());

            }
        }
    }
}