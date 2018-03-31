using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected.Test
{
    public class LoggingAdapter : IMemberLogger
    {
        readonly List<string> logger;
        readonly MemberTag memberTag;
        public LoggingAdapter(MemberTag memberTag, List<string> logger)
        {
            this.logger = logger;
            this.memberTag = memberTag;
        }

        public void Input(DateTime dateTime, object input)
        {
            logger.Add($"{memberTag.GetCategory()} input " + input?.ToString());
        }

        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            logger.Add($"{memberTag.GetCategory()} finish " + isSuccess.ToString());
        }

        public void LogActivityStart(DateTime dateTime)
        {
            logger.Add($"{memberTag.GetCategory()} start ");
        }

        public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
        {
            foreach (var v in verboseMessages)
            {
                logger.Add($"{memberTag.GetCategory()} Verbose Buffered " + v.Message);
            }
        }

        public void LogError(DateTime dateTime, string message)
        {
            logger.Add($"{memberTag.GetCategory()} excepion " + message);
        }

        public void LogException(DateTime dateTime, Exception exception)
        {
            logger.Add($"{memberTag.GetCategory()} excepion " + exception.ToString());
        }

        public void LogVerbose(DateTime dateTime, string message)
        {
            logger.Add($"{memberTag.GetCategory()} Verbose " + message);
        }

        public void Output(DateTime dateTime, object output)
        {
            logger.Add($"{memberTag.GetCategory()} output " + output?.ToString());
        }
    }
}
