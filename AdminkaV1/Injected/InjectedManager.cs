using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.SqlServer;

using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.AdminkaV1.Injected.Logging;

using DashboardCode.AdminkaV1.LoggingDom.WcfClient;

#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
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
        #region Meta
        public readonly static IStorageMetaService StorageMetaService = new StorageMetaService(
            (exception, storageModel, genericErrorField) => StorageResultBuilder.AnalyzeExceptionRecursive(
                  exception, storageModel, genericErrorField,
                  (ex, storageResultBuilder) => {
                      DataAccessEfCoreManager.Analyze(ex, storageResultBuilder);
                      SqlServerManager.Analyze(ex, storageResultBuilder);
                  }
            )
        );
        #endregion

        public static IIdentity GetDefaultIdentity()
        {
#if (NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
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

        public static string Markdown(this Exception exception, Action<StringBuilder, Exception> appender = null)
        {
            string text = ExceptionExtensions.Markdown(exception,
                (sb, ex) =>
                {
                    if (ex is AdminkaException)
                        sb.AppendUserContextException((AdminkaException)ex);
                    WcfClientManager.AppendWcfClientFaultException(sb, ex);
                    DataAccessEfCoreManager.Append(sb, ex);
                    SqlServerManager.Append(sb, ex);
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
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
        public static string SerializeToXml<T>(T t, Include<T> include) where T: class
        {
            var knownTypes = include.ListLeafTypes();
            return SerializeToXml(t, typeof(T), knownTypes);
        }
        public static string SerializeToXml(object o, IEnumerable<Type> knownTypes=null)
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

#region Active directory
        public static IEnumerable<string> GetGroups(this IIdentity identity, out string identityName, out string givenName, out string surname)
        {
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD2_0)
            var groups = ActiveDirectoryManager.ListGroups(identity, out identityName, out givenName, out surname);
                return groups;
#else
            throw new NotImplementedException("LDAP is not supported for NETStandard");
#endif
        }
#endregion

#region Logging
        public static Exception DefaultRoutineTagTransformException(Exception exception, RoutineGuid routineGuid, Func<Exception, string> markdownException)
        {
            exception.Data[nameof(RoutineGuid.CorrelationToken)]  = routineGuid.CorrelationToken;
            exception.Data[nameof(MemberTag.Namespace)]           = routineGuid.MemberTag.Namespace;
            exception.Data[nameof(MemberTag.Type)]                = routineGuid.MemberTag.Type;
            exception.Data[nameof(MemberTag.Member)]              = routineGuid.MemberTag.Member;
            return exception;
        }
        internal static NLogAuthenticationLogging GetNLogAuthenticationLogging()
        {
            return new NLogAuthenticationLogging();
        }
        public static Func<RoutineGuid, IContainer, RoutineLoggingTransients> ComposeNLogTransients(
                Func<Exception, string> markdownException,
                Func<Exception, RoutineGuid, Func<Exception, string>, Exception> routineTransformException
            )
        {
            return (t, r) => {
                var loggingConfiguration = r.Resolve<LoggingConfiguration>();
                var loggingPerformanceConfiguration = r.Resolve<LoggingPerformanceConfiguration>();
                var loggingVerboseConfiguration = r.Resolve<LoggingVerboseConfiguration>();
                var adminkaLogging = new NLogLoggingAdapter(
                        t,
                        markdownException,
                        SerializeToJson,
                        loggingConfiguration,
                        loggingVerboseConfiguration,
                        loggingPerformanceConfiguration
                    );
                var authenticationLogging = GetNLogAuthenticationLogging();
                return new RoutineLoggingTransients(adminkaLogging, authenticationLogging, (ex) => routineTransformException(ex, t, markdownException));
            };
        }
        public static Func<RoutineGuid, IContainer, RoutineLoggingTransients> ComposeListLoggingTransients(
            List<string> logger,
            LoggingConfiguration loggingConfiguration,
            LoggingVerboseConfiguration loggingVerboseConfiguration,
            LoggingPerformanceConfiguration loggingPerformanceConfiguration
            )
        {
            return (t, r) =>
            {
                Func<Exception, string> markdownException = Markdown;
                var adminkaLogging = new LoggingToListAdapter(
                    logger,
                    t,
                    markdownException,
                    SerializeToJson,
                    loggingConfiguration,
                    loggingVerboseConfiguration,
                    loggingPerformanceConfiguration
                    );
                return new RoutineLoggingTransients(adminkaLogging, adminkaLogging,
                    ex => DefaultRoutineTagTransformException(ex, t, markdownException));
            };
        }
        #endregion


        public static AdminkaDbContext CreateAdminkaDbContext(
            IAdmikaConfigurationFacade  admikaConfigurationFacade,
            MemberTag memberTag, 
            UserContext userContext,
            bool hasVerbose = false,
            Action<DateTime, string> verbose = null
            )
        {
            var routineGuid = new RoutineGuid(memberTag);
            var configurationContainerFactory = new ContainerFactory(admikaConfigurationFacade);
            var container = configurationContainerFactory.CreateContainer(routineGuid, userContext);

            // NOTE : alternative how to build verbose logger
            if (hasVerbose && verbose == null)
            {
                Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory =
                    ComposeNLogTransients(Markdown, DefaultRoutineTagTransformException);
                var routineLoggingTransients = loggingTransientsFactory(routineGuid, container);
                var basicLogging = routineLoggingTransients.BasicRoutineLoggingAdapter;
                var bufferedVerboseLoggingAdapter = new Routines.Injected.BufferedVerboseLogging(
                    basicLogging,
                    basicLogging.LogBufferedVerbose,
                    basicLogging.ShouldVerboseWithStackTrace
                    );
                verbose = bufferedVerboseLoggingAdapter.LogVerbose;
            }

            var closure = new RoutineClosure<UserContext>(userContext, routineGuid, hasVerbose?verbose:null, container);
            var adminkaStorageConfiguration = admikaConfigurationFacade.ResolveAdminkaStorageConfiguration();
            var adminkaDbContextFactory = new AdminkaDbContextFactory(adminkaStorageConfiguration);
            var adminkaDbContext = adminkaDbContextFactory.Create(closure);

            //  NOTE :  alternative how to build the same quick way
            //  var routine = new AdminkaRoutineHandler(new MemberTag(this), userContext, installerApplicationFactory, new { });
            //  return routine.Handle(
            //    (closure, adminkaDbContextFactory) => {
            //        var dbContext = adminkaDbContextFactory.Resolve();
            //        return dbContext;
            //    });

            return adminkaDbContext;
        }
    }
}