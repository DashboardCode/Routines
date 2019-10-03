using System;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore
{
    public class AuthenticationDomDataAccessEfCoreException : AdminkaException
    {
        public readonly RoutineClosure<UserContext> closure;
        public AuthenticationDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, Exception ex, string code="USER"):base(message, ex, code) =>
            this.closure = closure;

        public AuthenticationDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, string code) : base(message, code) =>
            this.closure = closure;
    }
}