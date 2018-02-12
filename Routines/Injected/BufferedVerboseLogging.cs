using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public class RoutineLogger
    {
        internal readonly VerboseBuffer buffer = new VerboseBuffer();
        private readonly RoutineGuid RoutineGuid;
        public RoutineLogger(RoutineGuid routineGuid) =>
            RoutineGuid = routineGuid;
    }

    internal class VerboseBuffer
    {
        private readonly ConcurrentQueue<VerboseBufferItem> buffer= new ConcurrentQueue<VerboseBufferItem>();
        //private readonly List<VerboseBufferItem> inputBuffer = new List<VerboseBufferItem>(1);
        //private readonly List<VerboseBufferItem> outputBuffer = new List<VerboseBufferItem>(1);

        public void Input(DateTime dateTime, object o) =>
            Add(dateTime, VerboseBufferItemType.Input, null, o, null);

        public void Output(DateTime dateTime, object o) =>
            Add(dateTime, VerboseBufferItemType.Output, null, o, null);

        public void LogVerbose(DateTime dateTime, string message, bool verboseWithStackTrace)
        {
            StackTraceProvider stackTraceProvider = null;
            if (verboseWithStackTrace)
                stackTraceProvider = new StackTraceProvider(new System.Diagnostics.StackTrace(2, true));
            Add(dateTime, VerboseBufferItemType.Verbose, message, null, stackTraceProvider);
        }

        private void Add(DateTime dateTime, VerboseBufferItemType itemType, string verboseMessage, object inputOutput, StackTraceProvider stackTraceProvider) =>
            buffer.Enqueue(new VerboseBufferItem(dateTime, itemType, verboseMessage, inputOutput, stackTraceProvider));

        public void Flash(IDataLogger verboseLogging, Action<List<VerboseMessage>> logBufferedVerbose)
        {
            var list = new List<VerboseMessage>();
            while (buffer.TryDequeue(out VerboseBufferItem message))
            {
                switch (message.ItemType)
                {
                    case VerboseBufferItemType.Input:
                        verboseLogging.Input(message.DateTime, message.InputOutput);
                        break;
                    case VerboseBufferItemType.Output:
                        verboseLogging.Output(message.DateTime, message.InputOutput);
                        break;
                    default:
                        list.Add(new VerboseMessage(message.DateTime, message.VerboseMessage, message.StackTrace));
                        break;
                }
            }
            logBufferedVerbose(list);
        }
    }

    public class BufferedVerboseLogging : IDataLogger
    {
        private readonly IDataLogger verboseLogging;
        private readonly bool verboseWithStackTrace;
        private readonly Action<List<VerboseMessage>> logBufferedVerbose;

        private readonly RoutineLogger routineLogger;
        private readonly object lockKey= new object();
        public BufferedVerboseLogging(
            IDataLogger verboseLogging,
            Action<List<VerboseMessage>> logBufferedVerbose,
            bool verboseWithStackTrace,
            RoutineLogger routineLogger)
        {
            this.verboseLogging = verboseLogging;
            this.logBufferedVerbose = logBufferedVerbose;
            this.verboseWithStackTrace = verboseWithStackTrace;
            this.routineLogger = routineLogger;
        }

        public void Input(DateTime dateTime, object o) =>
            routineLogger.buffer.Input(dateTime, o);

        public void Output(DateTime dateTime, object o) =>
            routineLogger.buffer.Output(dateTime, o);

        public void LogVerbose(DateTime dateTime, string message) =>
            routineLogger.buffer.LogVerbose(dateTime, message, verboseWithStackTrace);

        public void Flash() =>
            routineLogger.buffer.Flash(verboseLogging, logBufferedVerbose);
    }


    enum VerboseBufferItemType { Verbose = 0, Input = 1, Output = 2 };
    class VerboseBufferItem
    {
        public readonly DateTime DateTime;
        public readonly string VerboseMessage;
        public readonly object InputOutput;
        public readonly VerboseBufferItemType ItemType;
        public readonly StackTraceProvider StackTrace;
        public VerboseBufferItem(DateTime dateTime, VerboseBufferItemType itemType, string verboseMessage, object inputOutput, StackTraceProvider stackTrace)
        {
            DateTime = dateTime;
            ItemType = itemType;
            VerboseMessage = verboseMessage;
            InputOutput = inputOutput;
            StackTrace = stackTrace;
        }
    }

    public class StackTraceProvider 
    {
        readonly System.Diagnostics.StackTrace stackTrace;

        public StackTraceProvider(System.Diagnostics.StackTrace stackTrace) =>
            this.stackTrace = stackTrace;

        public string GetText() =>
           stackTrace.ToString();
    }

    public class VerboseMessage
    {
        public readonly DateTime   DateTime;
        public readonly string     Message;
        public readonly StackTraceProvider StackTrace;

        public VerboseMessage(DateTime dateTime, string text, StackTraceProvider stackTrace)
        {
            DateTime = dateTime;
            Message = text;
            StackTrace = stackTrace;
        }
    }
}