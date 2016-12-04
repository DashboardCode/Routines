using Vse.Routines;

namespace Vse.AdminkaV1.Injected
{
    public interface IAuthenticationLogging 
    {
        void TraceAuthentication(RoutineTag routineTag, string message);
    }
}
