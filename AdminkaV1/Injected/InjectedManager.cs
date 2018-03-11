using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;

using DashboardCode.Routines;
using DashboardCode.Routines.Injected;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.LoggingDom.WcfClient;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;

using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.ActiveDirectoryServices;
using DashboardCode.AdminkaV1.Injected.Performance;

#if !NETSTANDARD2_0
    using System.Security.Principal;
    using DashboardCode.Routines.Serialization.NETFramework;
    using DashboardCode.Routines.ActiveDirectory.NETFramework;
#else
    using System.Security.Principal;
    using DashboardCode.Routines.Serialization.NETStandard;
#endif

namespace DashboardCode.AdminkaV1.Injected
{
    public static class InjectedManager
    {
        static InjectedManager()
        {
            ForceEarlyFail();
        }

        /// <summary>
        /// Force early fails of application in case of missed assemblies (FileNotFoundException, Could not load file or assembly..)
        /// Those problems are quite common since transition and collection of references doesn't work well.
        /// </summary>
        private static void ForceEarlyFail()
        {
#if !NETSTANDARD2_0
            /* 
              System.ServiceModel.Primitives, 4.4.1 case. Can be diagnosed by unit case: AdminkaV1.Injected.SqlServer.NETFramework.Test 
              Important: when NUGET informs that System.ServiceModel.Primitives 4.4.1 version installed, actually 4.2.0.0 specified 
              as version number, therefore app.config bindingRedirect should be adjusted - pointed to 4.2.0.0 (or just deleted, sicne default 
              bindingRedirect "no bindingRedirect" works well in such cases) 
            */
              
              WcfClientManager.AppendWcfClientFaultException(new StringBuilder(), new Exception());
#endif
        }

        #region Meta
        public readonly static IEntityMetaServiceContainer EntityMetaServiceContainer = new EntityMetaServiceContainer(
            (exception, entityType, ormEntitySchemaAdapter, genericErrorField) => StorageResultBuilder.AnalyzeExceptionRecursive(
                  exception, entityType, ormEntitySchemaAdapter, genericErrorField,
                  (ex, storageResultBuilder) => {
                      DataAccessEfCoreManager.Analyze(ex, storageResultBuilder);
                      SqlServerManager.Analyze(ex, storageResultBuilder);
                  }
            )
        ); 
        #endregion

        public static IIdentity GetDefaultIdentity()
        {
#if NETSTANDARD2_0
            // TODO: Core 2.1 will contains AD functionality https://github.com/dotnet/corefx/issues/2089 and 
            // there we will need update this code to get roles similar to WindowsIdentity.GetCurrent().

            return new GenericIdentity(Environment.UserDomainName + "\\" + Environment.UserName, "Anonymous");
#else
            return WindowsIdentity.GetCurrent(); 
#endif
        }

#region Exception
        public static string ToHtml(this Exception exception)
        {
            string text = Markdown(exception);
            var markdown = new HeyRed.MarkdownSharp.Markdown();

            string html1 = markdown.Transform(text);
            var html2 = html1.Replace("<code>\n   at ", "<code><br />at ");

            return html2;
        }

        public static string Markdown(this Exception exception)
        {
            string text = Markdown(exception, null);
            return text;
        }

        private static string Markdown(this Exception exception, Action<StringBuilder, Exception> appender = null)
        {
            string text = ExceptionExtensions.Markdown(exception,
                (sb, ex) =>
                {
                    if (ex is AdminkaException)
                        sb.AppendUserContextException((AdminkaException)ex);
                    WcfClientManager.AppendWcfClientFaultException(sb, ex);
                    DataAccessEfCoreManager.Append(sb, ex);
                    SqlServerManager.Append(sb, ex);
#if !(NETSTANDARD2_0)
                    ActiveDirectoryManager.Append(sb, ex);
#endif
                    appender?.Invoke(sb, ex);
                }
            );
            return text;
        }

        private static void AppendUserContextException(this StringBuilder stringBuilder, AdminkaException exception)
        {
            var userContextException = exception;
            stringBuilder.AppendMarkdownLine(nameof(AdminkaException) + " specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("Code", userContextException.Code);
        }
#endregion

#region Serialize
        public static T DeserializeXml<T>(string xmlText, Include<T> include = null) where T : class
        {
            var knownTypes = include.ListLeafTypes();
            return DeserializeXml<T>(xmlText, knownTypes);
        }
        public static string SerializeToXml<T>(T t, Include<T> include) where T : class
        {
            var knownTypes = include.ListLeafTypes();
            return SerializeToXml(t, typeof(T), knownTypes);
        }
        public static string SerializeToXml(object o, IEnumerable<Type> knownTypes = null)
        {
            return SerializeToXml(o, o.GetType(), knownTypes);
        }
        public static T DeserializeXml<T>(string xmlText, IEnumerable<Type> knownTypes = null)
        {
            return SerializationManager.DeserializeXml<T>(xmlText, knownTypes);
        }
        private static string SerializeToXml(object o, Type rootType, IEnumerable<Type> knownTypes = null)
        {
            return SerializationManager.SerializeToXml(o, rootType, knownTypes);
        }
        public static T DeserializeJson<T>(string json)
        {
            return SerializationManager.DeserializeJson<T>(json);
        }
        public static string SerializeToJson(object o)
        {
            return SerializationManager.SerializeToJson(o);
        }
        public static string SerializeToJson(object o, int depth, bool ignoreDuplicates)
        {
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
            var types = Assembly.GetAssembly(typeof(UserContext)).GetTypes();
                return SerializationManager.SerializeToJson(o, depth, ignoreDuplicates, types);
#else
            var types = typeof(UserContext).GetTypeInfo().Assembly.GetTypes();
            return SerializationManager.SerializeToJson(o, depth, ignoreDuplicates, types);
#endif
        }
        public static string Markdown(string text)
        {
            var m = new HeyRed.MarkdownSharp.Markdown();
            var html = m.Transform(text);
            return html;
        }
#endregion

#region Logging
        public static Exception DefaultRoutineTagTransformException(
            Exception exception, 
            Guid correlationToken,
            MemberTag memberTag, 
            Func<Exception, string> markdownException)
        {
            exception.Data["CorrelationToken"] = correlationToken;
            exception.Data[nameof(MemberTag.Namespace)] = memberTag.Namespace;
            exception.Data[nameof(MemberTag.Type)] = memberTag.Type;
            exception.Data[nameof(MemberTag.Member)] = memberTag.Member;
            return exception;
        }

        private static readonly RoutineHandlerFactory<RoutineClosure<UserContext>> RoutineHandlerFactory = new RoutineHandlerFactory<RoutineClosure<UserContext>>(
            (ticks) => {; });

        public static Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> ComposeNLogTransients(
                Func<Exception, Guid, MemberTag, Func<Exception, string>, Exception> routineTransformException
            )
        {
            return (routineLogger, memberTag, containerFactory, userContext, input) => {
                var container = containerFactory.CreateContainer(memberTag, userContext);
                var loggingConfiguration = container.Resolve<LoggingConfiguration>();

                var nlogLoggingAdapter = new NLogLoggingAdapter(
                        routineLogger.CorrelationToken,
                        memberTag,
                        Markdown,
                        SerializeToJson,
                        loggingConfiguration
                    );

                var activityLogger = (IActivityLogger)nlogLoggingAdapter;
                var exceptionLogger = (IExceptionLogger)nlogLoggingAdapter;
                var verboseLogger = (IVerboseLogger)nlogLoggingAdapter;
                var dataLogger = (IDataLogger)nlogLoggingAdapter;
                var bufferedVerboseLogger = (IBufferedVerboseLogger)nlogLoggingAdapter;

                Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true; // get from container

                var loggingVerboseConfiguration = container.Resolve<LoggingVerboseConfiguration>();
                var shouldBufferVerbose = loggingVerboseConfiguration.ShouldBufferVerbose;
                var exceptionHandler = new ExceptionHandler(
                   ex => exceptionLogger.LogException(DateTime.Now, ex),
                   ex => DefaultRoutineTagTransformException(ex, routineLogger.CorrelationToken, memberTag, Markdown)
                );

                var (routineHandler, closure) = RoutineHandlerFactory.CreateRoutineHandler(
                    shouldBufferVerbose,
                    (verbose) => new RoutineClosure<UserContext>(userContext, verbose, container),
                    exceptionHandler,
                    loggingConfiguration.FinishActivity,
                    input, 
                    activityLogger, dataLogger,
                    bufferedVerboseLogger, 
                    routineLogger.buffer, 
                    loggingVerboseConfiguration.ShouldVerboseWithStackTrace, testInputOutput
                    );

                Action<string> efDbContextVerbose=null;
                if (shouldBufferVerbose)
                {
                    var loggerProviderConfiguration = closure.Resolve<LoggerProviderConfiguration>();
                    efDbContextVerbose = (loggerProviderConfiguration.Enabled) ? closure.Verbose : null;
                }

                var authenticationLogging = new NLogAuthenticationLogging();
                var routineLoggingTransients = new RoutineLoggingTransients(
                    authenticationLogging,
                    routineHandler,
                    efDbContextVerbose
                );

                return routineLoggingTransients;
            };
        }

        public static Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext, object, RoutineLoggingTransients> ComposeListLoggingTransients(
            List<string> logger,
            LoggingConfiguration loggingConfiguration = null,
            LoggingVerboseConfiguration loggingVerboseConfiguration = null,
            LoggingPerformanceConfiguration loggingPerformanceConfiguration = null
            )
        {
            return (routineLogger, memberTag, containerFactory, userContext, input) =>
            {
                var loggingConfiguration2 = loggingConfiguration ?? new LoggingConfiguration();
                var loggingVerboseConfiguration2 = loggingVerboseConfiguration ?? new LoggingVerboseConfiguration();
                var listLoggingAdapter = new ListLoggingAdapter(
                    logger,
                    routineLogger.CorrelationToken,
                    memberTag,
                    Markdown,
                    SerializeToJson,
                    loggingConfiguration2,
                    loggingPerformanceConfiguration ?? new LoggingPerformanceConfiguration()
                );

                var activityLogger = (IActivityLogger)listLoggingAdapter;
                var exceptionLogger = (IExceptionLogger)listLoggingAdapter;
                var verboseLogger = (IVerboseLogger)listLoggingAdapter;
                var dataLogger = (IDataLogger)listLoggingAdapter;
                var bufferedVerboseLogger = (IBufferedVerboseLogger)listLoggingAdapter;
                var authenticationLogging = (IAuthenticationLogging)listLoggingAdapter;

                

                var container = containerFactory.CreateContainer(memberTag, userContext);
                Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true; // get from container

                var exceptionHandler = new ExceptionHandler(
                   ex => exceptionLogger.LogException(DateTime.Now, ex),
                   ex => DefaultRoutineTagTransformException(ex, routineLogger.CorrelationToken, memberTag, Markdown)
                );
                var shouldBufferVerbose = loggingVerboseConfiguration2.ShouldBufferVerbose;
                var (routineHandler, closure) = RoutineHandlerFactory.CreateRoutineHandler(
                    loggingVerboseConfiguration2.ShouldBufferVerbose,
                    (verbose) => new RoutineClosure<UserContext>(userContext, verbose, container),
                    exceptionHandler,
                    loggingConfiguration2.FinishActivity,
                    input, 
                    activityLogger, dataLogger,
                    bufferedVerboseLogger,
                    routineLogger.buffer, 
                    loggingVerboseConfiguration2.ShouldVerboseWithStackTrace, testInputOutput
                    );

                Action<string> efDbContextVerbose = null;
                if (shouldBufferVerbose)
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
            };
        }


#endregion



        class JsonDeserializer : IGFactory<string>
        {
            public T Create<T>(string json) =>
                DeserializeJson<T>(json);
        }
        static readonly JsonDeserializer jsonDeserializer = new JsonDeserializer();

        public static string GetVerboseLoggingFlag(UserContext userContext) =>
            (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;
        

        public static ContainerFactory<UserContext> CreateContainerFactory(IConfigurationContainerFactory configurationFactory) =>
            new ContainerFactory<UserContext>(
                configurationFactory,
                GetVerboseLoggingFlag,
                jsonDeserializer);

        public static IContainer CreateContainer(IConfigurationContainerFactory configurationFactory, MemberTag memberTag, UserContext userContext) =>
             new ContainerFactory<UserContext>(
                 configurationFactory,
                 GetVerboseLoggingFlag,
                 jsonDeserializer).CreateContainer(memberTag, userContext);

        public static UserContext GetUserContext(
                RoutineLogger routineLogger,
                Func<RoutineLogger, MemberTag, ContainerFactory<UserContext>, UserContext , object, RoutineLoggingTransients> loggingTransientsFactory,
                AdminkaStorageConfiguration adminkaStorageConfiguration,
                ContainerFactory<UserContext> containerFactory,
                MemberTag memberTag,
                IIdentity identity,
                CultureInfo cultureInfo
            )
        {
            var systemUserContext = new UserContext("Authentication");
            var memberTag2 = new MemberTag("InjectedManager", "GetUserContext");

            /// 1) it writes activity and verbose messages to log with its own MemberTag but with the parent's CorrelationToken, Guid.
            /// 2) subroutine uses the same configuration as parent routine but can "use different @for specification", means configuration can be overrided for "user"
            
            var routine = new AdminkaRoutineHandler(
                adminkaStorageConfiguration,
                routineLogger,
                loggingTransientsFactory,
                containerFactory,
                memberTag2,
                systemUserContext,
                new {
                    PrincipalAuthenticationType = identity.AuthenticationType,
                    PrincipalName = identity.Name,
                    PrincipalIsAuthenticated = identity.IsAuthenticated,
                    PrincipalType = identity.GetType().FullName,
                    CultureInfo = cultureInfo.ToString()
                });
            var user = routine.HandleServicesContainer((authenticationService, closure, logAuthentication) =>
            {
                try
                {
                    var adConfiguration = closure.Resolve<AdConfiguration>();
                    bool useAdAuthorization = adConfiguration.UseAdAuthorization;
                    closure?.Verbose($"useAdAuthorization={useAdAuthorization}");
                    User @value;
                    if (useAdAuthorization)
                    {
#if !NETSTANDARD2_0
                        var groups = ActiveDirectoryManager.ListGroups(identity, out string loginName, out string firstName, out string secondName);
                        @value = authenticationService.GetUser(loginName, firstName, secondName, groups);
#else
                        throw new NotImplementedException("LDAP is not supported for NETStandard");
#endif
                    }
                    else
                    {
                        var fakeAdConfiguration = closure.Resolve<FakeAdConfiguration>();
                        @value = authenticationService.GetUser(
                            fakeAdConfiguration.FakeAdUser, "Anonymous", "Anonymous", fakeAdConfiguration.FakeAdGroups);
                    }
                    logAuthentication(routineLogger.CorrelationToken, memberTag, @value.LoginName);
                    return @value;
                }
                catch (Exception ex)
                {
                    throw new AdminkaException("User authetication and authorization service generates an error because of configuration or      network connection problems", ex);
                }
            });
            return new UserContext(user, cultureInfo);
        }
    }
}