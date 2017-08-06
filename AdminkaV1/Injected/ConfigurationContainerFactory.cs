using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.AdminkaV1.DomAuthentication;
using DashboardCode.Routines;
using System;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ConfigurationContainerFactory
    {
        IApplicationFactory appilcationFactory;
        AdminkaStorageConfiguration adminkaStorageConfiguration;
        public ConfigurationContainerFactory(IApplicationFactory appilcationFactory)
        {
            this.appilcationFactory = appilcationFactory;
            this.adminkaStorageConfiguration = appilcationFactory.CreateAdminkaStorageConfiguration();
        }
        public IContainer CreateContainer(RoutineGuid routineGuid, UserContext userContext)
        {
            var specifyResolver = ComposeContainerFactory(routineGuid);
            var @value = specifyResolver(userContext);
            return @value;
        }
        public Func<UserContext, IContainer> ComposeContainerFactory(RoutineGuid routineGuid)
        {
            Func<UserContext, IContainer> specifyResolver = (userContext) =>
            {
                var @for = (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;
                var configurationContainer = appilcationFactory.ComposeSpecify(routineGuid.MemberTag, @for);
                return new Resolver(configurationContainer);
            };
            return specifyResolver;
        }
        public AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration() =>
            adminkaStorageConfiguration;

        public RepositoryHandlerFactory ResolveRepositoryHandlerFactory() =>
            new RepositoryHandlerFactory( adminkaStorageConfiguration, new StorageMetaService());
    }
}