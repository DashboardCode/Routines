using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Vse.Routines.Injected
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
        public void Input(DateTime dateTime, object o)
        {
            Add(dateTime, ItemType.Input, null, o, null);
        }
        public void Output(DateTime dateTime, object o)
        {
            Add(dateTime, ItemType.Output, null, o, null);
        }
        public void LogVerbose(DateTime dateTime, string message)
        {
            Add(dateTime, ItemType.Verbose, message, null, verboseWithStackTrace ? new StackTrace(1, true) : null);
        }
        private void Add(DateTime dateTime, ItemType itemType, string verboseMessage, object inputOutput, StackTrace stackTrace)
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
            public readonly StackTrace StackTrace;
            public VerboseBufferItem(DateTime dateTime, ItemType itemType, string verboseMessage, object inputOutput, StackTrace stackTrace)
            {
                DateTime = dateTime;
                ItemType = itemType;
                VerboseMessage = verboseMessage;
                InputOutput = inputOutput;
                StackTrace = stackTrace;
            }
        }
    }

    public class VerboseMessage
    {
        public readonly DateTime DateTime;
        public readonly string Message;
        public readonly StackTrace StackTrace;

        public VerboseMessage(DateTime dateTime, string text, StackTrace stackTrace)
        {
            DateTime = dateTime;
            Message = text;
            StackTrace = stackTrace;
        }
    }
}