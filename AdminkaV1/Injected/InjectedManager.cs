using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.AdminkaV1.DomAuthentication;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.AdminkaV1.Injected.Logging;

#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7)
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
        public static IIdentity GetDefaultIdentity()
        {
            return WindowsIdentity.GetCurrent();
        }
        #region Depends on configuration
        public static StorageMetaService GetStorageMetaService(IAppConfiguration configuration)
        {
            var connectionString = configuration.ResolveConnectionString();
            var migrationAssembly = configuration.ResolveMigrationAssembly();
            var storageType = configuration.ResolveStorageType();
            return new StorageMetaService(connectionString, migrationAssembly, storageType);
        }
        internal static IResolver GetSpecifiedResolver(this RoutineGuid routineGuid, UserContext userContext, IAppConfiguration configuration)
        {
            GetResolver(routineGuid, configuration, out Func<UserContext, IResolver> specifyResolver);
            var @value = specifyResolver(userContext);
            return @value;
        }
        internal static IResolver GetResolver(this RoutineGuid routineGuid, IAppConfiguration configuration, out Func<UserContext, IResolver> specifyResolver)
        {
            var specifieableConfigurationContainer = configuration.ResolveConfigurationContainer(routineGuid.MemberTag);
            specifyResolver = (userContext) => {
                var @for = (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;
                var configurationContainer = specifieableConfigurationContainer.Specify(@for);
                return new Resolver(configurationContainer);
            };

            var resolver = new Resolver(specifieableConfigurationContainer);
            return resolver;
        }
        #endregion
        #region Exception
        public static List<FieldError> Analyze(this Exception exception, StorageModel storageModel)
        {
            var list = StorageErrorExtensions.AnalyzeException(exception,
                  (ex, l) => {
                      DataAccessEfCoreManager.Analyze(ex, l, storageModel);
                      SqlServerManager.Analyze(ex, l, storageModel);
                  }
            );
            return list;
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
                    if (ex is UserContextException)
                        sb.AppendUserContextException((UserContextException)ex);
                    DataAccessEfCoreManager.Append(sb, ex);
                    SqlServerManager.Append(sb, ex);
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7)
                    ActiveDirectoryManager.Append(sb, ex);
                    #endif
                    appender?.Invoke(sb, ex);
                }
            );
            return text;
        }
        private static void AppendUserContextException(this StringBuilder stringBuilder, UserContextException exception)
        {
            if (exception is UserContextException)
            {
                var userContextException = exception;
                stringBuilder.AppendMarkdownLine(nameof(UserContextException) + " specific:");
                stringBuilder.Append("   ").AppendMarkdownProperty("Code", userContextException.Code);
            }
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
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7)
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
#if !(NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7)
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
        public static Func<RoutineGuid, IResolver, RoutineLoggingTransients> ComposeNLogTransients(
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
        public static Func<RoutineGuid, IResolver, RoutineLoggingTransients> ComposeListLoggingTransients(
            List<string> logger,
            LoggingConfiguration loggingConfiguration,
            LoggingVerboseConfiguration loggingVerboseConfiguration,
            LoggingPerformanceConfiguration loggingPerformanceConfiguration
            )
        {
            return (t, r) =>
            {
                Func<Exception, string> markdownException = Markdown;
                var adminkaLogging = new ListLoggingAdapter(
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
    }
}