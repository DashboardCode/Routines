using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public class BufferedVerboseLogging : IVerboseLogging
    {
        private readonly IVerboseLogging loggingAdapter;
        private readonly bool verboseWithStackTrace;
        private readonly Action<List<VerboseMessage>> logBufferedVerbose;

        // Note: I can't imagine verbose logging so heavy that it will requre parallel ConcurentBag.Add
        private readonly List<VerboseBufferItem> buffer = new List<VerboseBufferItem>();
        private readonly object lockKey= new object();
        public BufferedVerboseLogging(
            IVerboseLogging loggingAdapter,
            Action<List<VerboseMessage>> logBufferedVerbose,
            bool verboseWithStackTrace)
        {
            this.loggingAdapter = loggingAdapter;
            this.logBufferedVerbose = logBufferedVerbose;
            this.verboseWithStackTrace = verboseWithStackTrace;
        }

        public void Input(DateTime dateTime, object o) =>
            Add(dateTime, ItemType.Input, null, o, null);

        public void Output(DateTime dateTime, object o) =>
            Add(dateTime, ItemType.Output, null, o, null);

        public void LogVerbose(DateTime dateTime, string message)
        {
            IStackTraceProvider stackTrace = null;
            if (verboseWithStackTrace)
#if NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0
                stackTrace = new StdStackTraceProvider(Environment.StackTrace);
#else
                stackTrace = new NfStackTraceProvider(new System.Diagnostics.StackTrace(1, true));
#endif
            Add(dateTime, ItemType.Verbose, message, null, stackTrace);
        }

        private void Add(DateTime dateTime, ItemType itemType, string verboseMessage, object inputOutput, IStackTraceProvider stackTrace)
        {
            lock (lockKey)
            {
                buffer.Add(new VerboseBufferItem(dateTime, itemType, verboseMessage, inputOutput, stackTrace));
            }
        }

        public void Flash()
        {
            var list = new List<VerboseMessage>();
            lock (lockKey)
            {
                foreach (var message in buffer)
                {
                    switch (message.ItemType)
                    {
                        case ItemType.Input:
                            loggingAdapter.Input(message.DateTime, message.InputOutput);
                            break;
                        case ItemType.Output:
                            loggingAdapter.Output(message.DateTime, message.InputOutput);
                            break;
                        default:
                            list.Add(new VerboseMessage(message.DateTime, message.VerboseMessage, message.StackTrace));
                            break;
                    }
                }
                logBufferedVerbose(list);
                buffer.Clear();
            }
        }

        private enum ItemType { Verbose=0, Input=1, Output=2};
        class VerboseBufferItem
        {
            public readonly DateTime DateTime;
            public readonly string VerboseMessage;
            public readonly object InputOutput;
            public readonly ItemType ItemType;
            public readonly IStackTraceProvider StackTrace;
            public VerboseBufferItem(DateTime dateTime, ItemType itemType, string verboseMessage, object inputOutput, IStackTraceProvider stackTrace)
            {
                DateTime = dateTime;
                ItemType = itemType;
                VerboseMessage = verboseMessage;
                InputOutput = inputOutput;
                StackTrace = stackTrace;
            }
        }
    }

    /// <summary>
    /// Wait for .NET Standard 2.0 there should be 
    /// </summary>
    public interface IStackTraceProvider
    {
        string GetText();
    }

#if NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0
    public class StdStackTraceProvider : IStackTraceProvider
    {
        readonly string stackTrace;

        public StdStackTraceProvider(string stackTrace) =>
            this.stackTrace = stackTrace;

        public string GetText() =>
           stackTrace;
    }
#else
    public class NfStackTraceProvider : IStackTraceProvider
    {
        readonly System.Diagnostics.StackTrace stackTrace;
        public NfStackTraceProvider(System.Diagnostics.StackTrace StackTrace) =>
            this.stackTrace = stackTrace;

        public string GetText() =>
           stackTrace.ToString();
    }
#endif
    public class VerboseMessage
    {
        public readonly DateTime   DateTime;
        public readonly string     Message;
        public readonly IStackTraceProvider StackTrace;

        public VerboseMessage(DateTime dateTime, string text, IStackTraceProvider stackTrace)
        {
            DateTime = dateTime;
            Message = text;
            StackTrace = stackTrace;
        }
    }
}