using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Logging
{
    public static class IMemberLoggerExtensions
    {
        class MemberLogger : IMemberLogger
        {
            private readonly IMemberLogger memberLogger1;
            private readonly IMemberLogger memberLogger2;
            public MemberLogger(IMemberLogger memberLogger1, IMemberLogger memberLogger2)
            {
                this.memberLogger1 = memberLogger1;
                this.memberLogger2 = memberLogger2;
            }
            

            public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
            {
                throw new NotImplementedException();
            }

            public void LogActivityStart(DateTime dateTime)
            {
                throw new NotImplementedException();
            }

            public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
            {
                throw new NotImplementedException();
            }

            public void LogError(DateTime dateTime, string message)
            {
                throw new NotImplementedException();
            }

            public void LogException(DateTime dateTime, Exception exception)
            {
                throw new NotImplementedException();
            }

            public void LogVerbose(DateTime dateTime, string message)
            {
                throw new NotImplementedException();
            }
            public void Input(DateTime dateTime, object input)
            {
                throw new NotImplementedException();
            }
            public void Output(DateTime dateTime, object output)
            {
                throw new NotImplementedException();
            }
        }

        public static IMemberLogger Add(IMemberLogger memberLogger1, IMemberLogger memberLogger2)
        {
            return new MemberLogger(memberLogger1, memberLogger2);
        }
    }
}
