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
    }
}