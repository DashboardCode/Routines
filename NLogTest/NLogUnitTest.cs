using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using NLog;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using NLog.Common;

namespace NLogTest
{
    // TODO: do it more realistic N users authenticate and trace activities, verbose episodically, 
    // when 1 user verbose constantly.
    [TestClass]
    public class NLogUnitTest
    {
        const int usersCount = 1;
        const int operations = 20;
        const int puaseBetweenOperationsMs = 0;
        const int bufferedMessageCountForOperation = 40;
        const int messageMinBytes = 1000;
        const int messageMaxBytes = 2000; // 2kb is a typewritten page
        const int repeats = 1;

        Logger logger1;
        Logger logger2;
        NLog.Targets.Target verboseTarget;
        public NLogUnitTest()
        {
            logger1 = LogManager.GetLogger("Routine1:Tests");
            logger2 = LogManager.GetLogger("Routine2:Tests");
            verboseTarget = LogManager.Configuration.FindTargetByName("verbose3");

        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public class VerboseMessage
        {
            public DateTime DateTime { get; set; }
            public string Message { get; set; }
        }

        public static List<VerboseMessage> Generate()
        {
            var list = new List<VerboseMessage>();
            for (var i =0; i< bufferedMessageCountForOperation; i++)
            {
                list.Add(new VerboseMessage() { DateTime = DateTime.Now, Message = RandomString( (i%2==0)? messageMaxBytes : messageMinBytes) });
            }
            return list;
        }

        [TestMethod]
        public void TestBufferingWrapper()
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
                                var verboseMessages = Generate();
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
        public void TestAsyncWrapper()
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
                                var verboseMessages = Generate();
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
        public void TestWriteAsyncLogEvents()
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
                                var verboseMessages = Generate();
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
                                            if (LogManager.ThrowExceptions && Thread.CurrentThread.ManagedThreadId == originalThreadId)
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
