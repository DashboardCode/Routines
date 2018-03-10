using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace DashboardCode.Routines.Injected
{
    public class VerboseBuffer
    {
        private readonly ConcurrentQueue<VerboseBufferItem> buffer = new ConcurrentQueue<VerboseBufferItem>();
        //private readonly List<VerboseBufferItem> inputBuffer = new List<VerboseBufferItem>(1);
        //private readonly List<VerboseBufferItem> outputBuffer = new List<VerboseBufferItem>(1);

        public void Input(DateTime dateTime, object o) =>
            Add(dateTime, VerboseBufferItemType.Input, null, o, null);

        public void Output(DateTime dateTime, object o) =>
            Add(dateTime, VerboseBufferItemType.Output, null, o, null);

        public void LogVerbose(DateTime dateTime, string message, bool verboseWithStackTrace)
        {
            StackTrace stackTrace = null;
            if (verboseWithStackTrace)
                stackTrace = new StackTrace(2, true);
            Add(dateTime, VerboseBufferItemType.Verbose, message, null, stackTrace);
        }

        private void Add(DateTime dateTime, VerboseBufferItemType itemType, string verboseMessage, object inputOutput, StackTrace stackTrace) =>
            buffer.Enqueue(new VerboseBufferItem(dateTime, itemType, verboseMessage, inputOutput, stackTrace));

        public void Flash(IDataLogger verboseLogging, Action<List<VerboseMessage>> logBufferedVerbose)
        {
            var list = new List<VerboseMessage>();
            while (buffer.TryDequeue(out VerboseBufferItem message))
            {
                switch (message.ItemType)
                {
                    case VerboseBufferItemType.Input:
                        verboseLogging.Input(message.DateTime, message.Data);
                        break;
                    case VerboseBufferItemType.Output:
                        verboseLogging.Output(message.DateTime, message.Data);
                        break;
                    default:
                        list.Add(new VerboseMessage(message.DateTime, message.Message, message.StackTrace));
                        break;
                }
            }
            logBufferedVerbose(list);
        }
    }
}