using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;

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
using DashboardCode.Routines.ActiveDirectory;
//using DashboardCode.Routines.Serialization;

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
#if !NETSTANDARD2_0
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
        //public static T DeserializeXml<T>(string xmlText, Include<T> include = null) where T : class
        //{
        //    var knownTypes = include.ListLeafTypes();
        //    return DeserializeXml<T>(xmlText, knownTypes);
        //}
        //public static string SerializeToXml<T>(T t, Include<T> include) where T : class
        //{
        //    var knownTypes = include.ListLeafTypes();
        //    return SerializeToXml(t, typeof(T), knownTypes);
        //}
        //public static string SerializeToXml(object o, IEnumerable<Type> knownTypes = null)
        //{
        //    return SerializeToXml(o, o.GetType(), knownTypes);
        //}
        //public static T DeserializeXml<T>(string xmlText, IEnumerable<Type> knownTypes = null)
        //{
        //    return SerializationManager.DeserializeXml<T>(xmlText, knownTypes);
        //}
        //private static string SerializeToXml(object o, Type rootType, IEnumerable<Type> knownTypes = null)
        //{
        //    return SerializationManager.SerializeToXml(o, rootType, knownTypes);
        //}
        public static T DeserializeJson<T>(string json)
        {
            return SerializationManager.DeserializeJson<T>(json);
        }
        class JsonDeserializerGFactory : IGFactory<string>
        {
            public T Create<T>(string json) =>
                DeserializeJson<T>(json);
        }
        static readonly JsonDeserializerGFactory jsonDeserializerGFactory = new JsonDeserializerGFactory();

        public static string SerializeToJson(object o)
        {
            return SerializationManager.SerializeToJson(o);
        }
        public static string SerializeToJson(object o, int depth, bool ignoreDuplicates)
        {
#if NETSTANDARD2_0
            var types = typeof(UserContext).GetTypeInfo().Assembly.GetTypes();
#else
            var types = Assembly.GetAssembly(typeof(UserContext)).GetTypes();
#endif
            return SerializationManager.SerializeToJson(o, depth, ignoreDuplicates, types);
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

        public static Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> ComposeNLogMemberLoggerFactory(IAuthenticationLogging authenticationLogging)
        {
            return (correlationToken, memberTag) => {
                var nLogLoggingAdapter = new NLogLoggingAdapter(correlationToken, memberTag, Markdown, SerializeToJson);
                return (nLogLoggingAdapter, authenticationLogging);
            };
        }

        public static Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> ComposeListMemberLoggerFactory(List<string> logger)
        {
            return (correlationToken, memberTag) => {
                var listLoggingAdapter = new ListLoggingAdapter(logger, correlationToken, memberTag, Markdown, SerializeToJson);
                return (listLoggingAdapter, listLoggingAdapter);
            };
        }
#endregion

        public static string GetVerboseLoggingFlag(UserContext userContext) =>
            (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;

        public static ContainerFactory<UserContext> CreateContainerFactory(ConfigurationContainerFactory configurationFactory) =>
            new ContainerFactory<UserContext>(
                configurationFactory,
                GetVerboseLoggingFlag,
                jsonDeserializerGFactory);

        public static Func<object, object, TimeSpan, bool> ComposeTestInputOutput(string errorRuleLang, string errorRule, Action<DateTime, string> logError)
        {
            if (string.IsNullOrEmpty(errorRuleLang) || string.IsNullOrEmpty(errorRule))
                return (input, output, TimeSpan) =>false;

            return (input, output, timeSpan) =>
            {
                var isAssert = false;
                try
                {
                    if (errorRuleLang == "DynamicExpresso")
                    {
                        var interpreter = new DynamicExpresso.Interpreter();
                        var result = interpreter.Eval(errorRule, new[] {
                            new DynamicExpresso.Parameter("input", input),
                            new DynamicExpresso.Parameter("output", output),
                            new DynamicExpresso.Parameter("timeSpan", timeSpan)
                        });
                        if (result is bool)
                            isAssert = (bool)result;
                        else
                        {
                            isAssert = true;
                            logError(DateTime.Now, $"Rule lang '{errorRuleLang}', rule '{errorRule}' has returned not boolean result");
                        }
                    }
                    if (!isAssert)
                        logError(DateTime.Now, $"[{errorRuleLang}] {errorRule}");
                }
                catch(Exception ex)
                {
                    isAssert = true;
                    logError(DateTime.Now, Markdown(ex));
                }
                return isAssert;
            };
        }

        public static UserContext GetUserContext(
                AdminkaRoutineLogger routineLogger,
                Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
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
                    closure.Verbose?.Invoke($"useAdAuthorization={useAdAuthorization}");
                    User @value;
                    if (useAdAuthorization)
                    {
#if NETSTANDARD2_0
                        throw new NotImplementedException("LDAP is not supported for NETStandard");
#else
                        var groups = ActiveDirectoryManager.ListGroups(identity, out string loginName, out string firstName, out string secondName);
                        @value = authenticationService.GetUser(loginName, firstName, secondName, groups);
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
                    throw new AdminkaException("User authetication and authorization service generates an error because of configuration or network connection problems", ex);
                }
            });
            return new UserContext(user, cultureInfo);
        }
    }
}