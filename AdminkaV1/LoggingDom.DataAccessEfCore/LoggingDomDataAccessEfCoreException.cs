using System;
using System.Runtime.Serialization;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore
{
    [Serializable]
    public class LoggingDomDataAccessEfCoreException : AdminkaException
    {
        public readonly RoutineClosure<UserContext> closure;
        public LoggingDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, Exception ex, string code="USER"):base(message, ex, code) =>
            this.closure = closure;

        public LoggingDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, string code) : base(message, code) =>
            this.closure = closure;

        protected LoggingDomDataAccessEfCoreException() : base()
        {
        }
        protected LoggingDomDataAccessEfCoreException(string message) : base(message)
        {
        }
        protected LoggingDomDataAccessEfCoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected LoggingDomDataAccessEfCoreException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}