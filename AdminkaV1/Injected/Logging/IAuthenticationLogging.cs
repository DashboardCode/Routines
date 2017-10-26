using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected
{
    public interface IAuthenticationLogging 
    {
        void TraceAuthentication(RoutineGuid routineGuid, string message);
    }
}