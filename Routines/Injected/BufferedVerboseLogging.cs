using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public class BufferedVerboseLogging : IDataLogger, IVerboseLogger
    {
        private readonly VerboseBuffer verboseBuffer;
        private readonly IDataLogger dataLogger;
        private readonly Action<List<VerboseMessage>> logBufferedVerbose;
        private readonly bool verboseWithStackTrace;

        public BufferedVerboseLogging(
            VerboseBuffer verboseBuffer,
            IDataLogger dataLogger,
            Action<List<VerboseMessage>> logBufferedVerbose,
            bool verboseWithStackTrace
            )
        {
            this.dataLogger = dataLogger;
            this.logBufferedVerbose = logBufferedVerbose;
            this.verboseWithStackTrace = verboseWithStackTrace;
            this.verboseBuffer = verboseBuffer;
        }

        public void Input(DateTime dateTime, object o) =>
            verboseBuffer.Input(dateTime, o);

        public void Output(DateTime dateTime, object o) =>
            verboseBuffer.Output(dateTime, o);

        public void LogVerbose(DateTime dateTime, string message) =>
            verboseBuffer.LogVerbose(dateTime, message, verboseWithStackTrace);

        public void Flash() =>
            verboseBuffer.Flash(dataLogger, logBufferedVerbose);
    }
}