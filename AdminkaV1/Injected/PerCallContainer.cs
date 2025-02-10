using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.LoggingDom.WcfClient;

#if NET9_0_OR_GREATER
using DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore;
using DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore.Services;

using DashboardCode.AdminkaV1.TestDom.DataAccessEfCore;
using DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore;

#endif

#if NET48
using DashboardCode.AdminkaV1.TestDom.DataAccessEf6;
using DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6;
using DashboardCode.AdminkaV1.LoggingDom.DataAccessEf6.Services;
#endif
namespace DashboardCode.AdminkaV1.Injected
{
    public class PerCallContainer<TUserContext>
    {
        private readonly RoutineClosure<TUserContext> closure;
        private readonly ApplicationSettings applicationSettings;
        private readonly Func<TUserContext, string> getAuditStamp;
        
        public PerCallContainer(
            RoutineClosure<TUserContext> closure,
            ApplicationSettings applicationSettings,
            Func<TUserContext, string> getAuditStamp)
        {
            this.closure = closure;
            this.applicationSettings = applicationSettings;
            this.getAuditStamp = getAuditStamp;
        }

#if NET9_0_OR_GREATER
        public LoggingDomStorageRoutineHandler<TUserContext> ResolveLoggingDomDbContextHandler()
        {
            var adminkaDbContextHandler = new LoggingDomStorageRoutineHandler<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return adminkaDbContextHandler;
        }

        public TestDomStorageRoutineHandler<TUserContext> ResolveTestDomDbContextHandler()
        {
            var testDomDbContextHandler = new TestDomStorageRoutineHandler<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return testDomDbContextHandler;
        }

        public AuthenticationDomStorageRoutineHandler<TUserContext> ResolveAuthenticationDomDbContextHandler()
        {
            var authenticationDomDbContextHandler = new AuthenticationDomStorageRoutineHandler<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return authenticationDomDbContextHandler;
        }

        // -------------

        public LoggingDomStorageRoutineHandlerAsync<TUserContext> ResolveLoggingDomDbContextHandlerAsync()
        {
            var adminkaDbContextHandler = new LoggingDomStorageRoutineHandlerAsync<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return adminkaDbContextHandler;
        }

        public TestDomStorageRoutineHandlerAsync<TUserContext> ResolveTestDomDbContextHandlerAsync()
        {
            var testDomDbContextHandler = new TestDomStorageRoutineHandlerAsync<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return testDomDbContextHandler;
        }

        public AuthenticationDomStorageRoutineHandlerAsync<TUserContext> ResolveAuthenticationDomDbContextHandlerAsync()
        {
            var authenticationDomDbContextHandler = new AuthenticationDomStorageRoutineHandlerAsync<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return authenticationDomDbContextHandler;
        }

#endif

#if NET48
        public LoggingDomStorageRoutineHandler<TUserContext> ResolveLoggingDomDbContextHandler()
        {
            var adminkaDbContextHandler = new LoggingDomStorageRoutineHandler<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return adminkaDbContextHandler;
        }

        public LoggingDomStorageRoutineHandlerAsync<TUserContext> ResolveLoggingDomDbContextHandlerAsync()
        {
            var adminkaDbContextHandler = new LoggingDomStorageRoutineHandlerAsync<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return adminkaDbContextHandler;
        }

        public TestDomStorageRoutineHandler<TUserContext> ResolveTestDomDbContextHandler()
        {
            var testDomDbContextHandler = new TestDomStorageRoutineHandler<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return testDomDbContextHandler;
        }

        public TestDomStorageRoutineHandlerAsync<TUserContext> ResolveTestDomDbContextHandlerAsync()
        {
            var testDomDbContextHandler = new TestDomStorageRoutineHandlerAsync<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return testDomDbContextHandler;
        }
#endif
        public ITraceServiceAsync ResolveTraceServiceAsyncWcf()
        {
            return new TraceServiceAsyncProxy(closure.Resolve<TraceServiceConfiguration>(), closure.Verbose);
        }

        public ITraceServiceAsync ResolveTraceServiceAsync()
        {
            return new TraceServiceAsync<TUserContext>(ResolveLoggingDomDbContextHandlerAsync());
        }

        public ITraceService ResolveTraceServiceWcf()
        {
            return new TraceServiceProxy(closure.Resolve<TraceServiceConfiguration>(), closure.Verbose);
        }

        public ITraceServiceAsync ResolveTraceService()
        {
            return new TraceServiceAsync<TUserContext>(ResolveLoggingDomDbContextHandlerAsync());
        }
    }
}
