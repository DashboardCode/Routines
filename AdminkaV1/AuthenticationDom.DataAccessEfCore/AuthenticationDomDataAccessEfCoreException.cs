using System;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore
{
    [Serializable]
    public class AuthenticationDomDataAccessEfCoreException : AdminkaException
    {
        public readonly RoutineClosure<UserContext> closure;
        public AuthenticationDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, Exception ex, string code="USER"):base(message, ex, code) =>
            this.closure = closure;

        public AuthenticationDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, string code) : base(message, code) =>
            this.closure = closure;

        protected AuthenticationDomDataAccessEfCoreException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }

        protected AuthenticationDomDataAccessEfCoreException():base()
        {
        }

        protected AuthenticationDomDataAccessEfCoreException(string message) : base(message)
        {
        }

        protected AuthenticationDomDataAccessEfCoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}