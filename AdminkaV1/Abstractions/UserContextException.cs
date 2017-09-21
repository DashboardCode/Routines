using System;
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
    using System.Runtime.Serialization;
#endif

namespace DashboardCode.AdminkaV1
{
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
    [Serializable]
#endif
    public class UserContextException : Exception
    {
        public readonly string Code;
        public UserContextException(string message, Exception ex, string code = "USER")
            : base(message, ex)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public UserContextException(string message, string code = "USER")
            : base(message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", Code);
        }
#endif
    }
}