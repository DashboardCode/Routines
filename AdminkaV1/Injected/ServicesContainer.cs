using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ServicesContainer
    {
        readonly AdminkaDbContextHandler dbContextManager;
        public ServicesContainer(DataAccessFactory dataAccessServices)
        {
            this.dbContextManager = dataAccessServices.CreateDbContextHandler();
        }

        internal ServicesContainer(AdminkaDbContextHandler dbContextManager)
        {
            this.dbContextManager = dbContextManager;
        }

        public ITraceService ResolveTraceService()
        {
            return new TraceService(dbContextManager);
        }

        public IAuthenticationService ResolveAuthenticationService()
        {
            return new DataAccessEfCore.Services.AuthenticationService(dbContextManager);
        }
    }
}