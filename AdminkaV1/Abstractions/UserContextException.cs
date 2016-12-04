using System;
using System.Runtime.Serialization;

namespace Vse.AdminkaV1
{
    [Serializable]
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", Code);
        }
    }
}
