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
    class NLogLoggingAdapter : IRoutineLogger
    {
        readonly Guid correlationToken;
        readonly MemberTag memberTag;
        readonly LoggingConfiguration loggingConfiguration;
        //readonly LoggingPerformanceConfiguration loggingPerformanceConfiguration;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, string> serializeObject;
        readonly Logger logger;
        
        public NLogLoggingAdapter(
            Guid correlationToken,
            MemberTag memberTag,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject,
            LoggingConfiguration loggingConfiguration//,
            //LoggingPerformanceConfiguration loggingPerformanceConfiguration
            )
        {
            this.correlationToken = correlationToken;
            this.memberTag = memberTag;
            this.markdownException = markdownException;

            this.serializeObject = (o)=> {
                try
                {
                    return serializeObject(o,1,true);
                }
                catch (Exception ex)
                {
                    LogException(DateTime.Now, ex);
                    return null;
                }
            };
            this.loggingConfiguration = loggingConfiguration;
            //this.loggingPerformanceConfiguration = loggingPerformanceConfiguration;
            var loggerName = "Routine:"+ memberTag.GetCategory();
            logger = LogManager.GetLogger(loggerName); // ~0.5 ms
        }

        public void LogActivityStart(DateTime dateTime)
        {
            if (loggingConfiguration.StartActivity)
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
        }

        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            if (loggingConfiguration.FinishActivity)
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

        public void LogBufferedVerbose(List<VerboseMessage> verboseMessages)
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
                if (verbose.StackTrace!=null)
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
            if (loggingConfiguration.Input)
            {
                var message = serializeObject(input);
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
        }

        public void Output(DateTime dateTime, object output)
        {
            if (loggingConfiguration.Output)
            {
                var message = serializeObject(output);
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
        }
    }
}