using System;
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
    using System.Runtime.Serialization;
#endif

namespace DashboardCode.AdminkaV1
{
    #if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
    [Serializable]
    #endif
    public class UserContextException : Exception
    {
        public readonly string Code;
        public UserContextException(string message, Exception ex, string code = "USER")
            : base(message, ex)
        {
            Code = code;
        }

        public UserContextException(string message, string code = "USER")
            : base(message)
        {
            Code = code;
        }
        #if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", Code);
        }
        #endif
    }
}
