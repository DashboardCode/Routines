using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected.Test
{
    public class LoggingTransients //: IBasicLoggingTransients
    {
        public IBasicLogging BasicRoutineLoggingAdapter { get; private set; }
        public Func<Exception, Exception> TransformException { get; private set; }

        public LoggingTransients(RoutineGuid routineGuid, List<string> log)
        {
            var loggingConfiguration = new LoggingConfiguration();
            BasicRoutineLoggingAdapter = new LoggingAdapter(routineGuid, log);
            TransformException = (ex) => ex;
        }
    }
}
