using System;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public interface IAuthenticationLogging 
    {
        void TraceAuthentication(Guid correlationToken, MemberTag memberTag, string message);
    }
}