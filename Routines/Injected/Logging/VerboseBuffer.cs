using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace DashboardCode.Routines.Injected.Logging
{
    public class VerboseBuffer
    {
        private readonly ConcurrentQueue<VerboseMessage> buffer = new ConcurrentQueue<VerboseMessage>();
        private readonly Action<IEnumerable<VerboseMessage>> logBufferedVerbose;
        private readonly bool shouldVerboseWithStackTrace;

        public VerboseBuffer(Action<IEnumerable<VerboseMessage>> logBufferedVerbose, bool shouldVerboseWithStackTrace)
        {
            this.logBufferedVerbose = logBufferedVerbose;
            this.shouldVerboseWithStackTrace = shouldVerboseWithStackTrace;
        }

        public void LogVerbose(DateTime dateTime, string message)
        {
            StackTrace stackTrace = null;
            if (shouldVerboseWithStackTrace)
                stackTrace = new StackTrace(2, true);
            buffer.Enqueue(new VerboseMessage(dateTime, message, stackTrace));
        }

        public void Flash() =>
            logBufferedVerbose(buffer);
    }
}