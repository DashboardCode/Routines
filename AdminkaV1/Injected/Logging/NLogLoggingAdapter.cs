using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using DashboardCode.Routines;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Injected;
using NLog.Common;
using System.Threading;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    class NLogLoggingAdapter : IBasicLogging
    {
        readonly RoutineTag routineTag;
        readonly LoggingConfiguration loggingConfiguration;
        readonly LoggingPerformanceConfiguration loggingPerformanceConfiguration;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, string> serializeObject;
        readonly Logger logger;

        public bool UseBufferForVerbose { get; private set; }
        public bool VerboseWithStackTrace { get; private set; }
        public NLogLoggingAdapter(
            RoutineTag routineTag,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject,
            LoggingConfiguration loggingConfiguration,
            LoggingVerboseConfiguration loggingVerboseConfiguration,
            LoggingPerformanceConfiguration loggingPerformanceConfiguration
            )
        {
            this.routineTag = routineTag;
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
            this.loggingPerformanceConfiguration = loggingPerformanceConfiguration;
            UseBufferForVerbose = loggingVerboseConfiguration.UseBufferForVerbose;
            VerboseWithStackTrace = loggingVerboseConfiguration.VerboseWithStackTrace;
            var loggerName = "Routine:"+ routineTag.GetCategory();
            logger = LogManager.GetLogger(loggerName); // ~0.5 ms
        }

        public void LogActivityStart(DateTime dateTime)
        {
            var logEventInfo = new LogEventInfo()
            {
                Level = LogLevel.Trace,
                TimeStamp = dateTime,
                Message = "Started"
            };
            logEventInfo.AppendRoutineTag(routineTag);
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
            logEventInfo.AppendRoutineTag(routineTag);
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
            logEventInfo.AppendRoutineTag(routineTag);
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
                logEventInfo.AppendRoutineTag(routineTag);
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
            logEventInfo.AppendRoutineTag(routineTag);
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
                logEventInfo.AppendRoutineTag(routineTag);
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
                logEventInfo.AppendRoutineTag(routineTag);
                logEventInfo.Properties["Description"] = $"Output";
                logger.Log(logEventInfo);
            }
        }

    }
}
