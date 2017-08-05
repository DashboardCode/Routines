using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected.Test
{
    public class LoggingAdapter : IBasicLogging
    {
        readonly List<string> logger;
        readonly RoutineGuid routineGuid;

        public LoggingAdapter(RoutineGuid routineGuid, List<string> logger)
        {
            this.logger = logger;
            this.routineGuid = routineGuid;
        }
        public void Input(DateTime dateTime, object input)
        {
            logger.Add($"{routineGuid.GetCategory()} input " + input?.ToString());
        }
        public void LogActivityFinish(DateTime dateTime, TimeSpan timeStamp, bool isSuccess)
        {
            logger.Add($"{routineGuid.GetCategory()} finish " + isSuccess.ToString());
        }
        public void LogActivityStart(DateTime dateTime)
        {
            logger.Add($"{routineGuid.GetCategory()} start ");
        }
        public void LogBufferedVerbose(List<VerboseMessage> verboseMessages)
        {
            foreach (var v in verboseMessages)
            {
                logger.Add($"{routineGuid.GetCategory()} Verbose Buffered " + v.Message);
            }
        }
        public void LogException(DateTime dateTime, Exception excepion)
        {
            logger.Add($"{routineGuid.GetCategory()} excepion " + excepion.ToString());
        }
        public void LogVerbose(DateTime dateTime, string message)
        {
            logger.Add($"{routineGuid.GetCategory()} Verbose " + message);
        }
        public void Output(DateTime dateTime, object output)
        {
            logger.Add($"{routineGuid.GetCategory()} output " + output?.ToString());
        }
        public bool ShouldBufferVerbose
        {
            get
            {
                return true;
            }
        }
        public bool ShouldVerboseWithStackTrace
        {
            get
            {
                return false;
            }
        }
    }
}
