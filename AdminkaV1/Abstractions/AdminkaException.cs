using System;
using System.Runtime.Serialization;

namespace DashboardCode.AdminkaV1
{
    [Serializable]
    public class AdminkaException : Exception
    {
        public readonly string Code;
        public AdminkaException(string message, Exception ex, string code = "USER")
            : base(message, ex)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public AdminkaException(string message, string code = "USER")
            : base(message)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", Code);
        }
    }
}