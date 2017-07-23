using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected.Test
{
    public class LoggingAdapter : IBasicLogging
    {
        readonly List<string> logger;
        readonly RoutineTag routineTag;

        public LoggingAdapter(RoutineTag routineTag, List<string> logger)
        {
            this.logger = logger;
            this.routineTag = routineTag;
        }
        public void Input(DateTime dateTime, object input)
        {
            logger.Add($"{routineTag.GetCategory()} input " + input?.ToString());
        }
        public void LogActivityFinish(DateTime dateTime, TimeSpan timeStamp, bool isSuccess)
        {
            logger.Add($"{routineTag.GetCategory()} finish " + isSuccess.ToString());
        }
        public void LogActivityStart(DateTime dateTime)
        {
            logger.Add($"{routineTag.GetCategory()} start ");
        }
        public void LogBufferedVerbose(List<VerboseMessage> verboseMessages)
        {
            foreach (var v in verboseMessages)
            {
                logger.Add($"{routineTag.GetCategory()} Verbose Buffered " + v.Message);
            }
        }
        public void LogException(DateTime dateTime, Exception excepion)
        {
            logger.Add($"{routineTag.GetCategory()} excepion " + excepion.ToString());
        }
        public void LogVerbose(DateTime dateTime, string message)
        {
            logger.Add($"{routineTag.GetCategory()} Verbose " + message);
        }
        public void Output(DateTime dateTime, object output)
        {
            logger.Add($"{routineTag.GetCategory()} output " + output?.ToString());
        }
        public bool UseBufferForVerbose
        {
            get
            {
                return true;
            }
        }
        public bool VerboseWithStackTrace
        {
            get
            {
                return false;
            }
        }
    }
}
