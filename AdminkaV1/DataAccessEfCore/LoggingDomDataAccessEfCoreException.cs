using System;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore
{
    public class LoggingDomDataAccessEfCoreException : AdminkaException
    {
        public readonly RoutineClosure<UserContext> closure;
        public LoggingDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, Exception ex, string code="USER"):base(message, ex, code) =>
            this.closure = closure;

        public LoggingDomDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, string code) : base(message, code) =>
            this.closure = closure;
    }
}