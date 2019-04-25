//using System;

//using DashboardCode.Routines;
//using DashboardCode.Routines.Logging;
//using DashboardCode.Routines.Configuration;

//using DashboardCode.AdminkaV1.Injected.Logging;
//using DashboardCode.AdminkaV1.Injected.Telemetry;

//namespace DashboardCode.AdminkaV1.Injected
//{
//    public class AdminkaRoutineLogger : RoutineHandlerFactory
//    {
//        readonly Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers;
//        readonly Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException;
//        readonly IPerformanceCounters performanceCounters;
//        public AdminkaRoutineLogger(
//            Guid correlationToken,
//            Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException,
//            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> composeLoggers,
//            IPerformanceCounters performanceCounters
//            ) : base(correlationToken)
//        {
//            this.composeLoggers = composeLoggers;
//            this.routineTransformException = routineTransformException;
//            this.performanceCounters = performanceCounters;
//        }

//        public RoutineLoggingTransients CreateTransients(
//            MemberTag memberTag,
//            ContainerFactory containerFactory,
//            UserContext userContext,
//            object input)
//        {
//            var container = containerFactory.CreateContainer(memberTag, userContext.AuditStamp);
//            var loggingConfiguration = container.Resolve<LoggingConfiguration>();
//            var loggingVerboseConfiguration = container.Resolve<LoggingVerboseConfiguration>();

//            var (memberLogger, authenticationLogging) = composeLoggers(base.CorrelationToken, memberTag);
//            var bufferedMemberLogger = base.CreateMemberLogger(memberLogger, 
//                loggingVerboseConfiguration.ShouldVerboseWithStackTrace,
//                loggingConfiguration.StartActivity);

//            var activityLogger = (IActivityLogger)bufferedMemberLogger;
//            var dataLogger = (IDataLogger)bufferedMemberLogger;
//            var exceptionLogger = (IExceptionLogger)bufferedMemberLogger;
//            var errorLogger = (IErrorLogger)bufferedMemberLogger;

//            Func<object, object, TimeSpan, bool> testInputOutput;
//            if (!loggingVerboseConfiguration.Input && !loggingVerboseConfiguration.Output && !loggingVerboseConfiguration.Verbose)
//                testInputOutput = (i, o, d) => false;
//            else
//                testInputOutput = InjectedManager.ComposeTestInputOutput(
//                    loggingVerboseConfiguration.ErrorRuleLang, 
//                    loggingVerboseConfiguration.ErrorRule, 
//                    (d,m)=> errorLogger.LogError(d,m));

//            var enableVerbose = loggingVerboseConfiguration.Verbose;
//            var exceptionHandler = new ExceptionHandler(
//               ex =>
//               {
//                   performanceCounters.CountError();
//                   exceptionLogger.LogException(DateTime.Now, ex);
//               },
//               ex => routineTransformException(ex, base.CorrelationToken, memberTag, InjectedManager.Markdown)
//            );

//            var (routineHandler, closure) = CreateRoutineHandler(
//                enableVerbose,
//                logVerbose => new RoutineClosure<UserContext>(userContext, logVerbose, container),
//                exceptionHandler,
//                loggingConfiguration.FinishActivity,
//                input,
//                activityLogger,
//                (dateTime, o) => dataLogger.Input(dateTime, o),
//                (dateTime, o) => dataLogger.Output(dateTime, o),
//                memberTag,
//                bufferedMemberLogger.LogVerbose,
//                loggingVerboseConfiguration.ShouldVerboseWithStackTrace,
//                testInputOutput,
//                performanceCounters.CountDurationTicks
//            );

//            Action<string> efDbContextVerbose = null;
//            if (enableVerbose)
//            {
//                var loggerProviderConfiguration = closure.Resolve<LoggerProviderConfiguration>();
//                efDbContextVerbose = (loggerProviderConfiguration.Enabled) ? closure.Verbose : null;
//            }

//            var routineLoggingTransients = new RoutineLoggingTransients(
//                authenticationLogging,
//                routineHandler,
//                efDbContextVerbose
//            );

//            return routineLoggingTransients;
//        }
//    }
//}