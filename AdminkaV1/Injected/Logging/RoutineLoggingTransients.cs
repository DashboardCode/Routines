using System;
using DashboardCode.Routines.Injected;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class RoutineLoggingTransients
    {
        public IAuthenticationLogging     AuthenticationLogging { get; private set; }
        public readonly ExceptionHandler ExceptionHandler;
        public readonly IRoutineLogging RoutineLogging;
        public readonly Action<DateTime, string> LogVerbose;

        public RoutineLoggingTransients(
            IAuthenticationLogging authenticationLogging,
            ExceptionHandler exceptionHandler,
            IRoutineLogging routineLogging,
            Action<DateTime, string> logVerbose
            )
        {
            AuthenticationLogging = authenticationLogging;
            ExceptionHandler = exceptionHandler;
            RoutineLogging = routineLogging;
            LogVerbose = logVerbose;
        }
    }
}