using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Injected;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class RoutineLoggingTransients
    {
        public IAuthenticationLogging AuthenticationLogging { get; private set; }
        public readonly IRoutineHandler<RoutineClosure<UserContext>> RoutineHandler;
        public readonly Action<string> Verbose;

        public RoutineLoggingTransients(
            IAuthenticationLogging authenticationLogging,
            IRoutineHandler<RoutineClosure<UserContext>> routineHandler,
            Action<string> verbose
            )
        {
            AuthenticationLogging = authenticationLogging;
            RoutineHandler = routineHandler;
            Verbose = verbose;
        }
    }
}