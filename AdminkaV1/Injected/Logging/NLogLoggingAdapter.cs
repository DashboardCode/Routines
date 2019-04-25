using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using NLog;
using NLog.Common;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Logging;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    class NLogLoggingAdapter : IMemberLogger
    {
        readonly Guid correlationToken;
        readonly ITraceDocumentBuilder documentBuilder;
        readonly Logger logger;
        readonly MemberTag memberTag;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, int, bool, string> serializeObject;

        public NLogLoggingAdapter(
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            MemberTag memberTag,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject
            )
        {
            this.correlationToken = correlationToken;
            this.documentBuilder = documentBuilder;
            this.memberTag = memberTag;
            var loggerName = "Routine:" + memberTag.GetCategory();
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
            logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Start;";
            if (documentBuilder != null)
            {
                documentBuilder.AddProperty(dateTime, "Started");
            }
            logger.Log(logEventInfo);
        }

        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            var verb = (isSuccess ? "Finished" : "FAILURE");
            var duration = Math.Round(timeSpan.TotalMilliseconds);

            var logEventInfo = new LogEventInfo()
            {
                Level = LogLevel.Trace,
                TimeStamp = dateTime,
                Message = verb + "; " + duration + "ms"
            };
            logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Finish";
            if (documentBuilder != null)
            {
                documentBuilder.AddProperty(dateTime, $"{verb} - {duration}ms");
            }
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
            logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Verbose";
            if (documentBuilder != null)
            {
                documentBuilder.AddVerbose(dateTime, message);
            }
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
                logEventInfo.AppendRoutineTag(verbose.DateTime, correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"BufferedVerbose";
                logEventInfo.Properties["Buffered"] = $"{i++}/{count}";
                if (verbose.StackTrace != null)
                    logEventInfo.Properties["StackTrace"] = verbose.StackTrace;
                if (documentBuilder != null)
                {
                    documentBuilder.AddVerbose(verbose.DateTime, verbose.Message);
                }
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
            logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Exception";
            if (documentBuilder != null)
            {
                documentBuilder.AddException(dateTime, message);
            }
            logger.Log(logEventInfo);
        }

        public void LogError(DateTime dateTime, string message)
        {
            var logEventInfo = new LogEventInfo()
            {
                Level = LogLevel.Error,
                TimeStamp = dateTime,
                Message = message
            };
            logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
            logEventInfo.Properties["Description"] = $"Error";
            if (documentBuilder != null)
            {
                documentBuilder.AddVerbose(dateTime, message);
            }
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
                logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"Input";
                if (documentBuilder != null)
                {
                    documentBuilder.AddInput(dateTime, message);
                }
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
                logEventInfo.AppendRoutineTag(dateTime, correlationToken, memberTag);
                logEventInfo.Properties["Description"] = $"Output";
                if (documentBuilder != null)
                {
                    documentBuilder.AddOutput(dateTime, message);
                }
                logger.Log(logEventInfo);
            }
            catch (Exception ex)
            {
                LogException(DateTime.Now, ex);
            }
        }
    }
}