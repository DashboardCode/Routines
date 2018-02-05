using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public interface IAuthenticationLogging 
    {
        void TraceAuthentication(RoutineGuid routineGuid, string message);
    }
}