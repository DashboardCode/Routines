using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NLog;
using netDumbster.smtp;

namespace DashboardCode.NLogExtensibles.Test
{
    // TODO: do it more realistic N users authenticate and trace activities, verbose episodically, 
    // when 1 user verbose constantly.
    [TestClass]
    public class NLogMailUnitTest
    {
        static SimpleSmtpServer server = SimpleSmtpServer.Start(25);

        const int usersCount = 2;
        const int operations = 3;
        const int puaseBetweenOperationsMs = 20;
        const int repeats = 3;

        [TestMethod]
        public void NLogMail()
        {
            var verboseLogger = NLog.LogManager.GetLogger("MailTest:"+nameof(NLogMailUnitTest));
            var c = 0;
            var verboseMessages = LogManager.Generate(20);
            var at = DateTime.Now;
            foreach (var verbose in verboseMessages)
            {
                var logEventInfo = new LogEventInfo()
                {
                    Level = LogLevel.Debug,
                    TimeStamp = verbose.DateTime,
                    Message = verbose.Message,
                };
                logEventInfo.Properties["LoggedAt"] = verbose.DateTime;
                logEventInfo.Properties["Number"] = c++;
                verboseLogger.Log(logEventInfo);
            }

            // https://github.com/cmendible/netDumbster
            // var smtpMessage = server.ReceivedEmail[0];
            // var body = smtpMessage.MessageParts[0].BodyData
            if (server.ReceivedEmailCount != 5)
                throw new ApplicationException("limited to 5 emails");
        }

        [TestMethod]
        public void NLogMailAssync()
        {
            var verboseLogger = NLog.LogManager.GetLogger("MailTest:" + nameof(NLogMailAssync));
            var c = 0;
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
                                var verboseMessages = LogManager.Generate();
                                var count = verboseMessages.Count();
                                //var i = 0;
                                var at = DateTime.Now;
                                foreach (var verbose in verboseMessages.Take(20))
                                {
                                    var number = Interlocked.Increment(ref c);
                                    var logEventInfo = new LogEventInfo()
                                    {
                                        Level = LogLevel.Debug,
                                        TimeStamp = verbose.DateTime,
                                        Message = verbose.Message,
                                    };
                                    logEventInfo.Properties["LoggedAt"] = verbose.DateTime;
                                    logEventInfo.Properties["Number"] = number;
                                    verboseLogger.Log(logEventInfo);

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
    }
}