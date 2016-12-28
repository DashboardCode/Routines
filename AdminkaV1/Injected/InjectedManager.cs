using System;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web.Script.Serialization;
using Vse.Web;
using Vse.Routines.Configuration;
using Vse.Routines;
using Vse.Routines.Storage;
using Vse.Routines.Storage.SqlServer;
using Vse.AdminkaV1.DomAuthentication;
using Vse.AdminkaV1.Injected.Configuration;
using Vse.AdminkaV1.Injected.Logging;
using Vse.AdminkaV1.DataAccessEfCore.Services;
using Vse.AdminkaV1.DataAccessEfCore;

namespace Vse.AdminkaV1.Injected
{
    public static class InjectedManager
    {
        public static StorageMetaService GetStorageMetaService()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["adminka"].ConnectionString;
            return new StorageMetaService(connectionString);
        }

        public static IIdentity GetDefaultIdentity()
        {
            return WindowsIdentity.GetCurrent();
        }
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
                    if (ex is LdapException)
                        sb.AppendLdapException((LdapException)ex);
                    if (ex is UserContextException)
                        sb.AppendUserContextException((UserContextException)ex);
                    DataAccessEfCoreManager.Append(sb, ex);
                    SqlServerManager.Append(sb, ex);
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
        private static void AppendLdapException(this StringBuilder stringBuilder, LdapException exception)
        {
            stringBuilder.AppendMarkdownLine("LdapException specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("ServerErrorMessage", exception.ServerErrorMessage);
            stringBuilder.Append("   ").AppendMarkdownProperty("ErrorCode", exception.ErrorCode.ToString());
            foreach (var partialResult in exception.PartialResults)
                if (partialResult != null)
                    stringBuilder.Append("   ").AppendMarkdownProperty("PartialResult", partialResult.ToString());
        }
        #endregion

        #region Serialize
        public static T DeserializeXml<T>(string xmlText, Include<T> include = null) where T : class
        {
            var knownTypes = MemberExpressionExtensions.GetTypes(include);
            return DeserializeXml<T>(xmlText, knownTypes);
        }
        public static T DeserializeXml<T>(string xmlText, IEnumerable<Type> knownTypes = null)
        {
            using (var stringReader = new StringReader(xmlText))
            {
                using (var xmlTextReader = new XmlTextReader(stringReader))
                {
                    var serializer = new DataContractSerializer(typeof(T), knownTypes);
                    var o = serializer.ReadObject(xmlTextReader);
                    var t = (T)o;
                    return t;
                }
            }
        }
        public static string SerializeToXml<T>(T t, Include<T> include) where T: class
        {
            var knownTypes = MemberExpressionExtensions.GetTypes(include);
            return SerializeToXml(t, typeof(T), knownTypes);
        }
        public static string SerializeToXml(object o, IEnumerable<Type> knownTypes=null)
        {
            return SerializeToXml(o, o.GetType(), knownTypes);
        }
        private static string SerializeToXml(object o, Type rootType, IEnumerable<Type> knownTypes = null)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    var serializer = new DataContractSerializer(rootType, knownTypes);
                    serializer.WriteObject(xmlTextWriter, o);
                    var text = stringWriter.ToString();
                    return text;
                }
            }
        }

        public static T DeserializeJson<T>(string json)
        {
            var serializer = new JavaScriptSerializer();
            var t = serializer.Deserialize<T>(json);
            return t;
        }
        public static string SerializeToJson(object o)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(o);
            return json;
        }
        public static string SerializeToJson(object o, int depth, bool ignoreDuplicates)
        {
            var serializer = new JavaScriptSerializer();
            var types = Assembly.GetAssembly(typeof(UserContext)).GetTypes();
            serializer.RegisterConverters(new[] { new CircularJsonConverter(types, MemberExpressionExtensions.SystemTypes, 30, false) });
            var json = serializer.Serialize(o);
            return json;
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
            var groups = new List<string>();
            identityName = identity.Name;
            givenName = null;
            surname = null;
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, identityName);
                givenName = userPrincipal.GivenName;
                surname = userPrincipal.Surname;
                var windowsIdentity = identity as WindowsIdentity;
                groups = windowsIdentity.Groups.Select(e => e.Translate(typeof(NTAccount)).Value).ToList();
            }
            return groups;
        }
        #endregion

        #region State's resolver 
        internal static IResolver GetResolver(this RoutineTag routineTag, out Func<UserContext, IResolver> specifyResolver)
        {
            var basicConfigurationContainer = RoutinesConfigurationManager.GetConfigurationContainer(routineTag.Namespace, routineTag.Class, routineTag.Member);
            specifyResolver = (userContext) =>
            {
                var @for = (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;
                var configurationContainer = basicConfigurationContainer.Specify(@for);
                return new Resolver(configurationContainer);
            };

            var resolver = new Resolver(basicConfigurationContainer);
            return resolver;
        }

        internal static IResolver GetSpecifiedResolver(this RoutineTag routineTag, UserContext userContext)
        {
            Func<UserContext, IResolver> specifyResolver;
            GetResolver(routineTag, out specifyResolver);
            var @value = specifyResolver(userContext);
            return @value;
        }
        #endregion

        #region Logging
        public static Exception DefaultRoutineTagTransformException(Exception exception, RoutineTag routineTag, Func<Exception, string> markdownException)
        {
            exception.Data[nameof(RoutineTag.CorrelationToken)] = routineTag.CorrelationToken;
            exception.Data[nameof(RoutineTag.Namespace)]        = routineTag.Namespace;
            exception.Data[nameof(RoutineTag.Class)]            = routineTag.Class;
            exception.Data[nameof(RoutineTag.Member)]           = routineTag.Member;
            return exception;
        }
        internal static NLogAuthenticationLogging GetNLogAuthenticationLogging()
        {
            return new NLogAuthenticationLogging();
        }
        public static Func<RoutineTag, IResolver, RoutineLoggingTransients> NLogConstructor
            (
                Func<Exception, string> markdownException,
                Func<Exception, RoutineTag, Func<Exception, string>, Exception> routineTransformException
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

        public static Func<RoutineTag, IResolver, RoutineLoggingTransients> ListConstructor(
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
