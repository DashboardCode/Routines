using System;
using System.Collections.Generic;
using System.Linq;
using Vse.AdminkaV1.Injected.Configuration;
using Vse.Routines.Storage;
using Vse.Routines;
using Vse.Routines.Injected;

namespace Vse.AdminkaV1.Injected.Logging
{
    public class ListLoggingAdapter : IBasicLogging, IAuthenticationLogging
    {
        readonly List<string> logger = new List<string>();
        readonly RoutineTag routineTag;
        readonly LoggingConfiguration loggingConfiguration;
        readonly LoggingPerformanceConfiguration loggingPerformanceConfiguration;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, string> serializeObject;
        public bool UseBufferForVerbose { get; private set; }
        public bool VerboseWithStackTrace { get; private set; }

        public ListLoggingAdapter(
            List<string> logger,
            RoutineTag routineTag,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject,
            LoggingConfiguration loggingConfiguration,
            LoggingVerboseConfiguration loggingVerboseConfiguration,
            LoggingPerformanceConfiguration loggingPerformanceConfiguration
            )
        {
            this.logger = logger;
            this.routineTag = routineTag;
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
            this.UseBufferForVerbose = loggingVerboseConfiguration.UseBufferForVerbose;
            this.VerboseWithStackTrace = loggingVerboseConfiguration.VerboseWithStackTrace;
        }


        public void LogActivityStart(DateTime dateTime)
        {
            var text = "LogActivityStart, " + dateTime +" "+ routineTag.ToString();
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }
        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            var text = "LogActivityFinish, " + dateTime + " " + routineTag.ToString()
                + " duration:"+((Math.Round(timeSpan.TotalMilliseconds) + "ms")) +(isSuccess ? "" : "; #ERROR");
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }
        public void LogVerbose(DateTime dateTime, string message)
        {
            var text = "LogVerbose, " + dateTime + " " + routineTag.ToString() + " message:" + message;
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }
        public void LogBufferedVerbose(List<VerboseMessage> verboseMessages)
        {
            var count = verboseMessages.Count();
            var i = 1;
            foreach (var verbose in verboseMessages)
            {
                var text = "LogBufferedVerbose,"+ $"{i++}/{count} " + verbose.DateTime + " " + routineTag.ToString() + " message:" + verbose.Message;
                
                if (verbose.StackTrace!=null)
                    text+=Environment.NewLine+ "StackTrace: " + verbose.StackTrace;
                //System.Diagnostics.Trace.WriteLine(text);
                logger.Add(text);
            }
        }

        public void LogException(DateTime dateTime, Exception excepion)
        {
            var message = markdownException(excepion);
            var text = "LogException, " + dateTime + " " + routineTag.ToString() + " message:" + Environment.NewLine + message;
            //System.Diagnostics.Trace.WriteLine(text);
            logger.Add(text);
        }

        public void Input(DateTime dateTime, object input)
        {
            if (loggingConfiguration.Input)
            {
                var message = serializeObject(input);
                var text = "Input, " + dateTime + " " + routineTag.ToString() + " message:" + message;
                //System.Diagnostics.Trace.WriteLine(text);
                logger.Add(text);
            }
        }

        public void Output(DateTime dateTime, object output)
        {
            if (loggingConfiguration.Output)
            {
                var message = serializeObject(output);
                var text = "Output, " + dateTime + " " + routineTag.ToString() + " message:" + message;
                //System.Diagnostics.Trace.WriteLine(text);
                logger.Add(text);
            }
        }

        public void TraceAuthentication(RoutineTag routineTag, string message)
        {
            
        }
    }
}
