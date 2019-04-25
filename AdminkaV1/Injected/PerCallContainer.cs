using System;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.LoggingDom.WcfClient;

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
        // TraceServiceHandler

        public MetaStorageRoutineHandler<TUserContext, AdminkaDbContext> ResolveAdminkaDbContextHandler()
        {
            var adminkaDbContextHandler = new AdminkaStorageRoutineHandler<TUserContext>(
                    applicationSettings.AdminkaStorageConfiguration,
                    closure.UserContext,
                    InjectedManager.EntityMetaServiceContainer,
                    null,
                    new Handler<RoutineClosure<TUserContext>, RoutineClosure<TUserContext>>(
                        () => closure,
                        closure
                    ),
                    getAuditStamp
                );
            return adminkaDbContextHandler;
        }

        public IHandler<ITraceService> ResolveTraceServiceHandler()
        {
            var traceServiceHandler = new HandlerUno<ITraceService>(
                    () => new TraceServiceProxy()
                );
            return traceServiceHandler;
        }
    }
}
