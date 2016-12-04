using Vse.AdminkaV1.DomLogging;
using Vse.AdminkaV1.DomAuthentication;
using Vse.AdminkaV1.DataAccessEfCore;
using Vse.AdminkaV1.DataAccessEfCore.Services;

namespace Vse.AdminkaV1.Injected
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
