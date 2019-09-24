using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Logging;
using DashboardCode.AdminkaV1.Injected.Telemetry;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class AdminkaRoutineHandlerFactory<TUserContext> : RoutineHandlerFactory
    { 
        readonly Func<Guid, MemberTag, IMemberLogger> composeLoggers;
        readonly Func<Exception, Guid, MemberTag, /*Func<Exception, string>,*/ Exception> routineTransformException;
        readonly IPerformanceCounters performanceCounters;
        public AdminkaRoutineHandlerFactory(
            Guid correlationToken,
            Func<Exception, Guid, MemberTag, /*Func<Exception, string>,*/ Exception> routineTransformException,
            Func<Guid, MemberTag, IMemberLogger> composeLoggers,
            IPerformanceCounters performanceCounters
            ) : base(correlationToken)
        {
            this.composeLoggers = composeLoggers;
            this.routineTransformException = routineTransformException;
            this.performanceCounters = performanceCounters;
        }

        public IHandler<RoutineClosure<TUserContext>> CreateLoggingHandler(
            MemberTag memberTag,
            IContainer container,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            object input)
        {
            var loggingConfiguration = container.Resolve<LoggingConfiguration>();
            var loggingVerboseConfiguration = container.Resolve<LoggingVerboseConfiguration>();

            var memberLogger = composeLoggers(base.CorrelationToken, memberTag);

            var bufferedMemberLogger = base.CreateMemberLogger(memberLogger,
                loggingVerboseConfiguration.ShouldVerboseWithStackTrace,
                loggingConfiguration.StartActivity);

            var activityLogger = (IActivityLogger)bufferedMemberLogger;
            var dataLogger = (IDataLogger)bufferedMemberLogger;
            var exceptionLogger = (IExceptionLogger)bufferedMemberLogger;
            var errorLogger = (IErrorLogger)bufferedMemberLogger;

            Func<object, object, TimeSpan, bool> testInputOutput;
            if (!loggingVerboseConfiguration.Input && !loggingVerboseConfiguration.Output && !loggingVerboseConfiguration.Verbose)
                testInputOutput = (i, o, d) => false;
            else
                testInputOutput = InjectedManager.ComposeTestInputOutput(
                    loggingVerboseConfiguration.ErrorRuleLang,
                    loggingVerboseConfiguration.ErrorRule,
                    (d, m) => errorLogger.LogError(d, m));

            var configFileEnableVerbose = loggingVerboseConfiguration.Verbose;
            var exceptionHandler = new ExceptionHandler(
               ex =>
               {
                   performanceCounters.CountError();
                   exceptionLogger.LogException(DateTime.Now, ex);
               },
               ex => routineTransformException(ex, base.CorrelationToken, memberTag/*, InjectedManager.Markdown*/)
            );
            var enableVerbose = hasVerboseLoggingPrivilege;
            if (!enableVerbose)
                enableVerbose = configFileEnableVerbose;
            var (routineHandler, closure) = CreateRoutineHandler(
                configFileEnableVerbose && hasVerboseLoggingPrivilege,
                logVerbose => new RoutineClosure<TUserContext>(userContext, logVerbose, container),
                exceptionHandler,
                loggingConfiguration.FinishActivity,
                input,
                activityLogger,
                (dateTime, o) => dataLogger.Input(dateTime, o),
                (dateTime, o) => dataLogger.Output(dateTime, o),
                memberTag,
                bufferedMemberLogger.LogVerbose,
                loggingVerboseConfiguration.ShouldVerboseWithStackTrace,
                testInputOutput,
                performanceCounters.CountDurationTicks
            );

            return routineHandler;
        }
    }
}
