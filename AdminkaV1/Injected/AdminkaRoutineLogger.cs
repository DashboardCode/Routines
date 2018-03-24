using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.Injected.Logging;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AdminkaRoutineLogger : RoutineLogger
    {
        readonly Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers;
        readonly Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException;
        public AdminkaRoutineLogger(
            Guid correlationToken,
            Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers
            ) : base(correlationToken)
        {
            this.composeLoggers = composeLoggers;
            this.routineTransformException = routineTransformException;
        }

        public RoutineLoggingTransients CreateTransients(
            MemberTag memberTag,
            ContainerFactory<UserContext> containerFactory,
            UserContext userContext,
            object input)
        {
            var container = containerFactory.CreateContainer(memberTag, userContext);
            var loggingConfiguration = container.Resolve<LoggingConfiguration>();
            var loggingVerboseConfiguration = container.Resolve<LoggingVerboseConfiguration>();

            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) =>
            {
                string flashBufferRuleText = loggingVerboseConfiguration.FlashBufferRule;
                if (string.IsNullOrEmpty(flashBufferRuleText))
                    return false;
                if (!loggingVerboseConfiguration.Input && !loggingVerboseConfiguration.Output && !loggingVerboseConfiguration.Verbose)
                    return false;
                //if ()
                return false;
            };

            var (memberLogger, authenticationLogging) = composeLoggers(base.CorrelationToken, memberTag);
            var bufferedMemberLogger = base.CreateMemberLogger(memberLogger, loggingVerboseConfiguration.ShouldVerboseWithStackTrace);

            var activityLogger = (IActivityLogger)bufferedMemberLogger;
            var dataLogger = (IDataLogger)bufferedMemberLogger;
            var exceptionLogger = (IExceptionLogger)bufferedMemberLogger;

            var enableVerbose = loggingVerboseConfiguration.Verbose;
            var exceptionHandler = new ExceptionHandler(
               ex => exceptionLogger.LogException(DateTime.Now, ex),
               ex => routineTransformException(ex, base.CorrelationToken, memberTag, InjectedManager.Markdown)
            );

            var (routineHandler, closure) = InjectedManager.RoutineHandlerFactory.CreateRoutineHandler(
                enableVerbose,
                logVerbose => new RoutineClosure<UserContext>(userContext, logVerbose, container),
                exceptionHandler,
                loggingConfiguration.FinishActivity,
                input,
                activityLogger,
                (dateTime, o) => dataLogger.Input(dateTime, o),
                (dateTime, o) => dataLogger.Output(dateTime, o),
                memberTag,
                bufferedMemberLogger.LogVerbose,
                base.Flash,
                loggingVerboseConfiguration.ShouldVerboseWithStackTrace,
                testInputOutput
            );

            Action<string> efDbContextVerbose = null;
            if (enableVerbose)
            {
                var loggerProviderConfiguration = closure.Resolve<LoggerProviderConfiguration>();
                efDbContextVerbose = (loggerProviderConfiguration.Enabled) ? closure.Verbose : null;
            }

            var routineLoggingTransients = new RoutineLoggingTransients(
                authenticationLogging,
                routineHandler,
                efDbContextVerbose
            );

            return routineLoggingTransients;
        }
    }
}
