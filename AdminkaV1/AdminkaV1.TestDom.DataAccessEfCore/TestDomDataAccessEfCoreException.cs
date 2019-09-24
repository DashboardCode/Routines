using System;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.TestDom.DataAccessEfCore
{
    public class AdminkaDataAccessEfCoreException : AdminkaException
    {
        public readonly RoutineClosure<UserContext> closure;
        public AdminkaDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, Exception ex, string code="USER"):base(message, ex, code) =>
            this.closure = closure;

        public AdminkaDataAccessEfCoreException(string message, RoutineClosure<UserContext> closure, string code) : base(message, code) =>
            this.closure = closure;
    }
}