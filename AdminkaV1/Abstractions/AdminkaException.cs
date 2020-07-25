using System;
using System.Runtime.Serialization;

namespace DashboardCode.AdminkaV1
{
    [Serializable]
    public class AdminkaException : Exception
    {
        public readonly string Code;

        protected AdminkaException(SerializationInfo info, StreamingContext context): base(info, context)
        { 

        }
        protected AdminkaException():base()
        {

        }

        protected AdminkaException(string message) : base(message)
        {

        }
        protected AdminkaException(string message, Exception innerException) : base(message, innerException)
        {

        }
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