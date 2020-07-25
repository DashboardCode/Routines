using System;
using System.Runtime.Serialization;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEfCore
{
    [Serializable]
    public class AdminkaDataAccessEfCoreException : AdminkaException
    {
        public readonly RoutineClosure<UserContext> closure;

        protected AdminkaDataAccessEfCoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        protected AdminkaDataAccessEfCoreException() : base()
        {

        }

        protected AdminkaDataAccessEfCoreException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected AdminkaDataAccessEfCoreException(string message) : base(message)
        {

        }
        public AdminkaDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, Exception ex, string code="USER"):base(message, ex, code) =>
            this.closure = closure;

        public AdminkaDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, string code) : base(message, code) =>
            this.closure = closure;
    }
}