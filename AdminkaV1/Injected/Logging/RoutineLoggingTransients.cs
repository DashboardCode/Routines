using System;
using DashboardCode.Routines.Injected;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class RoutineLoggingTransients
    {
        public IBasicLogging              BasicRoutineLoggingAdapter   { get; private set; }
        public IAuthenticationLogging     AuthenticationLoggingAdapter { get; private set; }
        public Func<Exception, Exception> TransformException           { get; private set; }
        public RoutineLoggingTransients(
            IBasicLogging adminkaLogging,
            IAuthenticationLogging specialLoggingAdapter,
            Func<Exception, Exception> transformException
            )
        {
            BasicRoutineLoggingAdapter = adminkaLogging;
            AuthenticationLoggingAdapter = specialLoggingAdapter;
            TransformException = transformException;
        }
    }
}
