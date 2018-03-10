using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public class RoutineLogger
    {
        public readonly VerboseBuffer buffer = new VerboseBuffer();
        public readonly Guid CorrelationToken;
        public RoutineLogger(Guid correlationToken) =>
            CorrelationToken = correlationToken;

        public BufferedVerboseLogging CreateBufferedVerboseLogging(IDataLogger dataLogger, Action<List<VerboseMessage>> logBufferedVerbose, bool shouldVerboseWithStackTrace)
        {
            var buffered = new BufferedVerboseLogging(
                           buffer,
                           dataLogger,
                           logBufferedVerbose,
                           shouldVerboseWithStackTrace
            );
            return buffered;
        }

        //public BufferedRoutineLogging CreateBufferedRoutineLogging(
        //    IDataLogger dataLogger, Action<List<VerboseMessage>> logBufferedVerbose, bool shouldVerboseWithStackTrace,
        //    ActivityState activityState, Func<object, object, TimeSpan, bool> testInputOutput)
        //{
        //    var buffered = CreateBufferedVerboseLogging(dataLogger, logBufferedVerbose, shouldVerboseWithStackTrace);
        //    var bufferedRoutineLogging = new BufferedRoutineLogging(
        //                activityState,
        //                buffered,
        //                buffered.LogVerbose,
        //                buffered.Flash,
        //                testInputOutput
        //    );
        //    return bufferedRoutineLogging;
        //}
        //public IRoutineLogger CreateRoutineLogger(MemberTag memberTag)
        //{
        //    var exceptionHandler = new ExceptionHandler(
        //            nlogLoggingAdapter.LogException,  // exception will writen directly (even if log is buffered, in this case after flash)
        //            ex => routineTransformException(ex, routineLogger.CorrelationToken, memberTag, Markdown)
        //        );
        //}
    }
}