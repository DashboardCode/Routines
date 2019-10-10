using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.LoggingDom.WcfClient;

#if NETSTANDARD2_1
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

#if NETSTANDARD2_1
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
#endif
        public ITraceService ResolveTraceServiceWcf()
        {
            return new TraceServiceProxy(closure.Resolve<TraceServiceConfiguration>(), closure.Verbose);
        }

        public ITraceService ResolveTraceService()
        {
            return new TraceService<TUserContext>(ResolveLoggingDomDbContextHandler());
        }

    }

}
