using System;
using System.Diagnostics;

namespace DashboardCode.Routines.Injected
{
    enum VerboseBufferItemType { Verbose = 0, Input = 1, Output = 2, ActivityStart=3, ActivityFinish = 4, Exception=5 };
    class VerboseBufferItem
    {
        public readonly DateTime DateTime;
        public readonly VerboseBufferItemType ItemType;
        public readonly string Message;
        public readonly object Data;
        public readonly StackTrace StackTrace;

        public VerboseBufferItem(DateTime dateTime, VerboseBufferItemType itemType, string message, object data, StackTrace stackTrace)
        {
            DateTime = dateTime;
            ItemType = itemType;
            Message = message;
            Data = data;
            StackTrace = stackTrace;
        }
    }
}