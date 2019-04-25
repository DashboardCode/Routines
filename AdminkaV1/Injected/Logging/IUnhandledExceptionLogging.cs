using System;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public interface IUnhandledExceptionLogging
    {
        void TraceError(Guid correlationToken, string message);
    }
}
