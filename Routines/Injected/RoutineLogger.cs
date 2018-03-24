using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Injected
{
    public class RoutineLogger
    {
        public readonly Guid CorrelationToken;
        private readonly List<VerboseBuffer> buffers = new List<VerboseBuffer>();
        //private readonly Func<Guid, MemberTag, IMemberLogger> createMemberLogger;
        public RoutineLogger(Guid correlationToken/*, Func<Guid, MemberTag,  IMemberLogger> createMemberLogger*/)
        {
            CorrelationToken = correlationToken;
            //this.createMemberLogger = createMemberLogger;
        }

        public void Flash()
        {
            foreach (var b in buffers)
                b.Flash();
        }
        
        public IMemberLogger CreateMemberLogger(IMemberLogger memberLogger/*MemberTag memberTag*/, bool shouldVerboseWithStackTrace)
        {
            //var memberLogger = createMemberLogger(CorrelationToken, memberTag);
            var buffer = new VerboseBuffer(memberLogger.LogBufferedVerbose, shouldVerboseWithStackTrace);
            buffers.Add(buffer);
            return new BufferedMemberLogger(memberLogger, buffer.LogVerbose);
        }

        class BufferedMemberLogger : IMemberLogger
        {
            readonly IMemberLogger memberLogger;
            readonly Action<DateTime, string> logVerbose;

            public BufferedMemberLogger(IMemberLogger memberLogger, Action<DateTime,string> logVerbose)
            {
                this.memberLogger = memberLogger;
                this.logVerbose = logVerbose;
            }

            public void Input(DateTime dateTime, object input)
            {
                memberLogger.Input(dateTime, input);
            }

            public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
            {
                memberLogger.LogActivityFinish(dateTime, timeSpan, isSuccess);
            }

            public void LogActivityStart(DateTime dateTime)
            {
                memberLogger.LogActivityStart(dateTime);
            }

            public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
            {
                memberLogger.LogBufferedVerbose(verboseMessages);
            }

            public void LogException(DateTime dateTime, Exception exception)
            {
                memberLogger.LogException(dateTime, exception);
            }

            public void LogVerbose(DateTime dateTime, string message)
            {
                logVerbose(dateTime, message);
            }

            public void Output(DateTime dateTime, object output)
            {
                memberLogger.Output(dateTime, output);
            }
        }
    }
}