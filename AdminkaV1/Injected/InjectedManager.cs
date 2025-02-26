﻿using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Principal;

using DashboardCode.Routines;

//using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.Routines.ActiveDirectory;
using DashboardCode.Routines.Logging;

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
#if !NET9_0_OR_GREATER
            /* 
              System.ServiceModel.Primitives, 4.4.1 case. Can be diagnosed by unit case: AdminkaV1.Injected.SqlServer.NETFramework.Test 
              Important: when NUGET informs that System.ServiceModel.Primitives 4.4.1 version installed, actually 4.2.0.0 specified 
              as version number, therefore app.config bindingRedirect should be adjusted - pointed to 4.2.0.0 (or just deleted, sicne default 
              bindingRedirect "no bindingRedirect" works well in such cases) 
            */
            // TODO test it again, for this I need new use case: wcf service should call other wcf service through client
            // WcfClientManager.AppendWcfClientFaultException(new StringBuilder(), new Exception());
#endif
        }

        public static IIdentity GetDefaultIdentity()
        {
#if NET9_0_OR_GREATER
            // TODO: Core 2.1 will contains AD functionality https://github.com/dotnet/corefx/issues/2089 and 
            // there we will need update this code to get roles similar to WindowsIdentity.GetCurrent().
            return new GenericIdentity(Environment.UserDomainName + "\\" + Environment.UserName, "Anonymous");
#else
            return WindowsIdentity.GetCurrent(); 
#endif
        }

        #region Exception
        public static string ToHtmlException(string exceptionMarkdown)
        {
            var html1 = ToHtml(exceptionMarkdown);
            var html2 = html1.Replace("<code>\n   at ", "<code><br />at ");
            return html2;
        }

        public static string ToHtml(this Exception exception)
        {
            string exceptionMarkdown = Markdown(exception);
            var html = ToHtmlException(exceptionMarkdown);
            return html;
        }

        public static string ToHtml(string mdText)
        {
            var markdown = new HeyRed.MarkdownSharp.Markdown();
            string html = markdown.Transform(mdText);
            return html;
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
#if NET9_0_OR_GREATER
                    DashboardCode.AdminkaV1.LoggingDom.WcfClient.WcfClientManager.AppendWcfClientFaultException(sb, ex);
                    DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore.LoggingDomDataAccessEfCoreManager.Append(sb, ex);
#endif

#if NET9_0_OR_GREATER
                    DashboardCode.Routines.Storage.SqlServer.SqlServerManager.Append(sb, ex);
#else
                    DashboardCode.Routines.Storage.SystemSqlServer.SqlServerManager.Append(sb, ex);
#endif
                    ActiveDirectoryManager.Append(sb, ex);
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

        public static string SerializeToJson(object o)
        {
            return SerializationManager.SerializeToJson(o);
        }
        public static string SerializeToJson(object o, int depth, bool ignoreDuplicates)
        {
#if NET9_0_OR_GREATER
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
            MemberTag memberTag
            //, 
            //Func<Exception, string> markdownException
            )
        {
            exception.Data["CorrelationToken"]          = correlationToken;
            exception.Data[nameof(MemberTag.Namespace)] = memberTag.Namespace;
            exception.Data[nameof(MemberTag.Type)]      = memberTag.Type;
            exception.Data[nameof(MemberTag.Member)]    = memberTag.Member;
            return exception;
        }

        public static Func<Guid, MemberTag, IMemberLogger> ComposeNLogMemberLoggerFactory(ITraceDocumentBuilder documentBuilder)
        {
            return (correlationToken, memberTag) => {
                var nLogLoggingAdapter = new NLogLoggingAdapter(correlationToken, documentBuilder, memberTag, Markdown, SerializeToJson);
                return nLogLoggingAdapter;
            };
        }

        public static Func<Guid, MemberTag, IMemberLogger> ComposeListMemberLoggerFactory(List<string> logger)
        {
            return (correlationToken, memberTag) => {
                var listLoggingAdapter = new ListLoggingAdapter(logger, correlationToken, memberTag, Markdown, SerializeToJson);
                return listLoggingAdapter;
            };
        }
        #endregion

        #region ApplicationSettings


        public static bool OperatingSystemIsWindows()
        {
#if NET9_0_OR_GREATER
            return OperatingSystem.IsWindows();
#else
            return false;
#endif
        }

#if NET9_0_OR_GREATER
        readonly static Routines.Configuration.Standard.DeserializerStandard deserializer = new Routines.Configuration.Standard.DeserializerStandard();
        public static ApplicationSettings CreateInMemoryApplicationSettingsStandard(string name)
        {
            //if (configurationRoot == null)
            //{
                var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
                Microsoft.Extensions.Configuration.JsonConfigurationExtensions.AddJsonFile(configurationBuilder, "appsettings.json", optional: false, reloadOnChange: true);
                Microsoft.Extensions.Configuration.IConfiguration configurationRoot = configurationBuilder.Build();
            //}
            var appSettings = new Routines.Configuration.Standard.AppSettings(configurationRoot);

            var configurationManagerLoader = new Routines.Configuration.Standard.ConfigurationManagerLoader(configurationRoot);
            var configurationContainerFactory = ResetConfigurationContainerFactoryStandard(configurationManagerLoader);
            var unhandledExceptionLogging = new NUnhandledExceptionLogging();
            return new ApplicationSettings(appSettings, configurationContainerFactory, unhandledExceptionLogging, new AdminkaStorageConfiguration(name, null, StorageType.INMEMORY, null));
        }

        
        public static IConfigurationContainerFactory ResetConfigurationContainerFactoryStandard(IConfigurationManagerLoader<Microsoft.Extensions.Configuration.IConfigurationSection> configurationManagerLoader)
        {
            return new ConfigurationContainerFactory<Microsoft.Extensions.Configuration.IConfigurationSection>(configurationManagerLoader, deserializer);
        }

        public static ApplicationSettings CreateApplicationSettingsStandard(Microsoft.Extensions.Configuration.IConfiguration configurationRoot=null, string migrationAssembly=null)
        {
            if (configurationRoot == null)
            {
                var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
                Microsoft.Extensions.Configuration.JsonConfigurationExtensions.AddJsonFile(configurationBuilder, "appsettings.json", optional: false, reloadOnChange: true);
                configurationRoot = configurationBuilder.Build();
            }
            var connectionStringMap = new Routines.Configuration.Standard.ConnectionStringMap(configurationRoot);
            var connectionString = connectionStringMap.GetConnectionString("AdminkaConnectionString");
            var adminkaStorageConfiguration = new AdminkaStorageConfiguration(connectionString, migrationAssembly, StorageType.SQLSERVER, migrationAssembly==null?default(int?):5*60);

            var appSettings = new Routines.Configuration.Standard.AppSettings(configurationRoot);

            var configurationManagerLoader = new Routines.Configuration.Standard.ConfigurationManagerLoader(configurationRoot);
            var configurationContainerFactory = ResetConfigurationContainerFactoryStandard(configurationManagerLoader);
            var unhandledExceptionLogging = new NUnhandledExceptionLogging();
            return new ApplicationSettings(appSettings, configurationContainerFactory, unhandledExceptionLogging, adminkaStorageConfiguration);
        }
#else
        readonly static Routines.Configuration.Classic.DeserializerClassic deserializerClassic = 
            new Routines.Configuration.Classic.DeserializerClassic(
                (j,o)=> Newtonsoft.Json.JsonConvert.SerializeObject(o)
                );
                public static IConfigurationContainerFactory ResetConfigurationContainerFactoryClassic()
        {
            return new ConfigurationContainerFactory<string>(new Routines.Configuration.Classic.ConfigurationManagerLoader(), deserializerClassic);
        }

        public static ApplicationSettings CreateInMemoryApplicationSettingsClassic(string name)
        {
            return 
                new ApplicationSettings(
                    new Routines.Configuration.Classic.AppSettings(),
                    ResetConfigurationContainerFactoryClassic(),
                    new NUnhandledExceptionLogging(),
                    new AdminkaStorageConfiguration(name, null, StorageType.INMEMORY, null)
                    );
        }

        public static ApplicationSettings CreateApplicationSettingsClassic()
        {
            var connectionStringMap = new Routines.Configuration.Classic.ConnectionStringMap();
            var connectionString = connectionStringMap.GetConnectionString("AdminkaConnectionString");
            var adminkaStorageConfiguration = new AdminkaStorageConfiguration(connectionString, null, StorageType.SQLSERVER, null);
            return new ApplicationSettings(
                new Routines.Configuration.Classic.AppSettings(), 
                ResetConfigurationContainerFactoryClassic(),
                new NUnhandledExceptionLogging(),
                adminkaStorageConfiguration
                );
        }
#endif

        public static ApplicationSettings CreateInMemoryApplicationSettings(string name)
        {
#if NET9_0_OR_GREATER
            return CreateInMemoryApplicationSettingsStandard(name);
#else
            return CreateInMemoryApplicationSettingsClassic(name);
#endif
        }

        public static ApplicationSettings CreateApplicationSettings()
        {
#if NET9_0_OR_GREATER
            return CreateApplicationSettingsStandard();
#else
            return CreateApplicationSettingsClassic();
#endif
        }


        #endregion

        public static ContainerFactory CreateContainerFactory(IConfigurationContainerFactory configurationFactory)
        {
            return
                new ContainerFactory(
                    configurationFactory);
        }

        public static string GetVerboseLoggingFlag(UserContext userContext) =>
            (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;


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

        //public static UserContext GetUserContext(
        //        AdminkaRoutineLogger routineLogger,
        //        Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
        //        AdminkaStorageConfiguration adminkaStorageConfiguration,
        //        ContainerFactory containerFactory,
        //        MemberTag memberTag,
        //        IIdentity identity,
        //        CultureInfo cultureInfo
        //    )
        //{
        //    var systemUserContext = new UserContext("Authentication");
        //    var memberTag2 = new MemberTag("InjectedManager", "GetUserContext");

        //    /// 1) it writes activity and verbose messages to log with its own MemberTag but with the parent's CorrelationToken, Guid.
        //    /// 2) subroutine uses the same configuration as parent routine but can "use different @for specification", means configuration can be overrided for "user"

        //    var routine = new AdminkaXRoutineHandler(
        //        adminkaStorageConfiguration,
        //        routineLogger,
        //        containerFactory,
        //        memberTag2,
        //        systemUserContext,
        //        new {
        //            PrincipalAuthenticationType = identity.AuthenticationType,
        //            PrincipalName = identity.Name,
        //            PrincipalIsAuthenticated = identity.IsAuthenticated,
        //            PrincipalType = identity.GetType().FullName,
        //            CultureInfo = cultureInfo.ToString()
        //        });
        //    var user = routine.HandleServicesContainer((authenticationService, closure, logAuthentication) =>
        //    {
        //        try
        //        {
        //            var adConfiguration = closure.Resolve<AdConfiguration>();
        //            bool useAdAuthorization = adConfiguration.UseAdAuthorization;
        //            closure.Verbose?.Invoke($"useAdAuthorization={useAdAuthorization}");
        //            User @value;
        //            if (useAdAuthorization)
        //            {
        //                var groups = ActiveDirectoryManager.ListGroups(identity, out string loginName, out string firstName, out string secondName);
        //                @value = authenticationService.GetUser(loginName, firstName, secondName, groups);
        //            }
        //            else
        //            {
        //                var fakeAdConfiguration = closure.Resolve<FakeAdConfiguration>();
        //                @value = authenticationService.GetUser(
        //                    fakeAdConfiguration.FakeAdUser, "Anonymous", "Anonymous", fakeAdConfiguration.FakeAdGroups);
        //            }
        //            logAuthentication(routineLogger.CorrelationToken, memberTag, @value.LoginName);
        //            return @value;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new AdminkaException("User authetication and authorization service generates an error because of configuration or network connection problems", ex);
        //        }
        //    });
        //    return new UserContext(user, cultureInfo);
        //}

#region DependencyInjection interface
        //public static void MvcConfigure(IApplicationBuilder applicationBuilder)
//        {
//            DatabaseErrorPageExtensions.UseDatabaseErrorPage(applicationBuilder);
//        }

        //public static void AddMvc(Microsoft.Extensions.Configuration.IConfigurationRoot configurationRoot, IServiceCollection serviceCollection)
//        {
//            var applicationSettings = CreateApplicationSettingsStandard(configurationRoot);
//            var connectionString = applicationSettings.AuthenticationStorageConfiguration.ConnectionString;

//            serviceCollection.AddDbContext<AuthenticationDbContext>(optionsBuilder =>
//                optionsBuilder.UseSqlServer(connectionString));

//            serviceCollection.AddScoped<IPasswordHasher<WebUser>, CustomPasswordHasher>();

//            serviceCollection.AddIdentity<WebUser, IdentityRole<int>>(

//                options =>
//                {
//                    options.SignIn.RequireConfirmedEmail = true;
//#if DEBUG
//                    options.Password.RequireDigit = false;
//                    options.Password.RequiredLength = 4;
//                    options.Password.RequireNonAlphanumeric = false;
//                    options.Password.RequireUppercase = false;
//                    options.Password.RequireLowercase = false;
//#endif   
//                })
//                .AddEntityFrameworkStores<AuthenticationDbContext>()
//                .AddDefaultTokenProviders();

//            serviceCollection.ConfigureApplicationCookie(options => options.LoginPath = new PathString("/WebUserIdentity/Account/Login"));

//            serviceCollection.AddSingleton(typeof(ApplicationSettings), applicationSettings);
//            serviceCollection.AddTransient<IEmailService, EmailService>();
//            serviceCollection.AddSingleton(typeof(ApplicationSettings), applicationSettings);
//        }

#endregion
    }
}