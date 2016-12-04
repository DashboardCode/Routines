using System;
using System.Collections.Generic;

namespace Vse.Routines.Injected.Test
{
    public class LoggingTransients //: IBasicLoggingTransients
    {
        public IBasicLogging BasicRoutineLoggingAdapter { get; private set; }
        public Func<Exception, Exception> TransformException { get; private set; }

        public LoggingTransients(RoutineTag routineTag, List<string> log)
        {
            var loggingConfiguration = new LoggingConfiguration();
            BasicRoutineLoggingAdapter = new LoggingAdapter(routineTag, log);
            TransformException = (ex) => ex;
        }
    }
}
