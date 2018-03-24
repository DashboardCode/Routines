using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using NLog;
using NLog.Common;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Injected;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    class NLogLoggingAdapter : IMemberLogger
    {
        readonly Guid correlationToken;
        readonly Logger logger;
        readonly MemberTag memberTag;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, int, bool, string> serializeObject;

        public NLogLoggingAdapter(
            Guid correlationToken,
            MemberTag memberTag,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject
            )
        {
            this.correlationToken = correlationToken;
            this.memberTag = memberTag;
            var loggerName = "Routine:"+ memberTag.GetCategory();
            logger = LogManager.GetLogger(loggerName); // ~0.5 ms
            this.serializeObject = serializeObject;
            this.markdownException = markdownException;
        }

        public void LogActivityStart(DateTime dateTime)
        {
                var logEventInfo = new LogEventInfo()
                {
                    Level = LogLevel.Trace,
                    TimeStamp = dateTime,
                    Message = "Started"
                };
                logEventInfo.AppendRoutineTag(correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"Start;";
                logger.Log(logEventInfo);
            }

        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
                var message = (isSuccess ? "Finished" : "FAILURE") + "; " +
                Math.Round(timeSpan.TotalMilliseconds) + "ms";

                var logEventInfo = new LogEventInfo()
                {
                    Level = LogLevel.Trace,
                    TimeStamp = dateTime,
                    Message = message
                };
                logEventInfo.AppendRoutineTag(correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"Finish";
                logger.Log(logEventInfo);
            
        }

        public void LogVerbose(DateTime dateTime, string message)
        {
            var logEventInfo = new LogEventInfo()
            {
                Level = LogLevel.Info,
                TimeStamp = dateTime,
                Message = message
            };
            logEventInfo.AppendRoutineTag(correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Verbose";
            logger.Log(logEventInfo);
        }

        public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
        {
            var count = verboseMessages.Count();
            var i = 1;
            var list = new List<AsyncLogEventInfo>();
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            foreach (var verbose in verboseMessages)
            {
                var logEventInfo = new LogEventInfo()
                {
                    Level = LogLevel.Info,
                    TimeStamp = verbose.DateTime,
                    Message = verbose.Message,
                };
                logEventInfo.AppendRoutineTag(correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"BufferedVerbose";
                logEventInfo.Properties["Buffered"] = $"{i++}/{count}";
                if (verbose.StackTrace != null)
                    logEventInfo.Properties["StackTrace"] = verbose.StackTrace;
                logger.Log(logEventInfo);
            }
        }
            

        public void LogException(DateTime dateTime, Exception excepion)
        {
            var message = markdownException(excepion);
            var logEventInfo = new LogEventInfo()
            {
                Level = LogLevel.Error,
                TimeStamp = dateTime,
                Message = message
            };
            logEventInfo.AppendRoutineTag(correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Exception";
            logger.Log(logEventInfo);
        }

        public void Input(DateTime dateTime, object input)
        {
            try
            {
                var message = serializeObject(input, 1, true);
                var logEventInfo = new LogEventInfo()
                {
                    Level = LogLevel.Info,
                    TimeStamp = dateTime,
                    Message = message
                };
                logEventInfo.AppendRoutineTag(correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"Input";
                logger.Log(logEventInfo);
            }
            catch (Exception ex)
            {
                LogException(DateTime.Now, ex);
            }
        }

        public void Output(DateTime dateTime, object output)
        {
            try
            {
                var message = serializeObject(output, 1, true);
                var logEventInfo = new LogEventInfo()
                {
                    Level = LogLevel.Info,
                    TimeStamp = dateTime,
                    Message = message
                };
                logEventInfo.AppendRoutineTag(correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"Output";
                logger.Log(logEventInfo);
            }
            catch (Exception ex)
            {
                LogException(DateTime.Now, ex);
            }
        }
    }
}