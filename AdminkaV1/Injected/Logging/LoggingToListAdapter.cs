using System;
using System.Collections.Generic;
using System.Linq;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.AdminkaV1.Injected.Performance;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingToListAdapter : IBasicLogging, IAuthenticationLogging
    {
        readonly List<string> logger = new List<string>();
        readonly RoutineGuid routineGuid;
        readonly LoggingConfiguration loggingConfiguration;
        readonly LoggingPerformanceConfiguration loggingPerformanceConfiguration;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, string> serializeObject;
        public bool ShouldBufferVerbose { get; private set; }
        public bool ShouldVerboseWithStackTrace { get; private set; }

        public LoggingToListAdapter(
            List<string> logger,
            RoutineGuid routineGuid,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject,
            LoggingConfiguration loggingConfiguration,
            LoggingVerboseConfiguration loggingVerboseConfiguration,
            LoggingPerformanceConfiguration loggingPerformanceConfiguration
            )
        {
            this.logger = logger;
            this.routineGuid = routineGuid;
            this.markdownException = markdownException;
            this.serializeObject = (o) => {
                try
                {
                    return serializeObject(o, 1, true);
                }
                catch (Exception ex)
                {
                    LogException(DateTime.Now, ex);
                    //return null;
                    throw;
                }
            };
            this.loggingConfiguration = loggingConfiguration;
            this.loggingPerformanceConfiguration = loggingPerformanceConfiguration;
            this.ShouldBufferVerbose = loggingVerboseConfiguration.ShouldBufferVerbose;
            this.ShouldVerboseWithStackTrace = loggingVerboseConfiguration.ShouldVerboseWithStackTrace;
        }


        public void LogActivityStart(DateTime dateTime)
        {
            var text = "LogActivityStart, " + dateTime +" "+ routineGuid.ToText();
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }
        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            var text = "LogActivityFinish, " + dateTime + " " + routineGuid.ToText()
                + " duration:"+((Math.Round(timeSpan.TotalMilliseconds) + "ms")) +(isSuccess ? "" : "; #ERROR");
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }
        public void LogVerbose(DateTime dateTime, string message)
        {
            var text = "LogVerbose, " + dateTime + " " + routineGuid.ToText() + " message:" + message;
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }
        public void LogBufferedVerbose(List<VerboseMessage> verboseMessages)
        {
            var count = verboseMessages.Count();
            var i = 1;
            foreach (var verbose in verboseMessages)
            {
                var text = "LogBufferedVerbose,"+ $"{i++}/{count} " + verbose.DateTime + " " + routineGuid.ToText() + " message:" + verbose.Message;
                
                if (verbose.StackTrace!=null)
                    text+=Environment.NewLine+ "StackTrace: " + verbose.StackTrace;
                //System.Diagnostics.Trace.WriteLine(text);
                logger.Add(text);
            }
        }

        public void LogException(DateTime dateTime, Exception excepion)
        {
            var message = markdownException(excepion);
            var text = "LogException, " + dateTime + " " + routineGuid.ToText() + " message:" + Environment.NewLine + message;
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }

        public void Input(DateTime dateTime, object input)
        {
            if (loggingConfiguration.Input)
            {
                var message = serializeObject(input);
                var text = "Input, " + dateTime + " " + routineGuid.ToText() + " message:" + message;
                //System.Diagnostics.Trace.WriteLine(text);
                logger.Add(text);
            }
        }

        public void Output(DateTime dateTime, object output)
        {
            if (loggingConfiguration.Output)
            {
                var message = serializeObject(output);
                var text = "Output, " + dateTime + " " + routineGuid.ToText() + " message:" + message;
                //System.Diagnostics.Trace.WriteLine(text);
                logger.Add(text);
            }
        }

        public void TraceAuthentication(RoutineGuid routineGuid, string message)
        {
            logger.Add($"{routineGuid.CorrelationToken} {RoutineGuidExtensions.ToText(routineGuid)} {message}");
        }
    }
}