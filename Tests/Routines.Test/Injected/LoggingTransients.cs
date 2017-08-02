using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected.Test
{
    public class LoggingTransients //: IBasicLoggingTransients
    {
        public IBasicLogging BasicRoutineLoggingAdapter { get; private set; }
        public Func<Exception, Exception> TransformException { get; private set; }

        public LoggingTransients(MemberGuid routineTag, List<string> log)
        {
            var loggingConfiguration = new LoggingConfiguration();
            BasicRoutineLoggingAdapter = new LoggingAdapter(routineTag, log);
            TransformException = (ex) => ex;
        }
    }
}
