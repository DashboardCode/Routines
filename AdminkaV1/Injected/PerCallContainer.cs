using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

#if NETSTANDARD2_1
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.LoggingDom.WcfClient;
using DashboardCode.AdminkaV1.TestDom.DataAccessEfCore;
using DashboardCode.AdminkaV1.AuthenticationDom.DataAccessEfCore;
using DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore;
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
        public MetaStorageRoutineHandler<TUserContext, LoggingDomDbContext> ResolveLoggingDomDbContextHandler()
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

        public MetaStorageRoutineHandler<TUserContext, TestDomDbContext> ResolveTestDomDbContextHandler()
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

        public MetaStorageRoutineHandler<TUserContext, AuthenticationDomDbContext> ResolveAuthenticationDomDbContextHandler()
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

        public IHandler<ITraceService> ResolveTraceServiceHandler()
        {
            var traceServiceHandler = new HandlerUno<ITraceService>(
                    () => new TraceServiceProxy()
                );
            return traceServiceHandler;
        }
#endif
    }

}
