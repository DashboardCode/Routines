using System;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected
{
    public class ContainerFactory
    {
        IAdmikaConfigurationFactory adminkaConfigurationFactory;
        public ContainerFactory(IAdmikaConfigurationFactory adminkaConfigurationFactory)=>
            this.adminkaConfigurationFactory = adminkaConfigurationFactory;

        public Func<UserContext, IContainer> ComposeContainerFactory(RoutineGuid routineGuid)
        {
            Func<UserContext, IContainer> specifyResolver = (userContext) =>
            {
                var @for = (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null;
                var configurationContainer = adminkaConfigurationFactory.ComposeSpecify(routineGuid.MemberTag, @for);
                return new Container(configurationContainer);
            };
            return specifyResolver;
        }

        public IContainer CreateContainer(RoutineGuid routineGuid, UserContext userContext)
        {
            var specifyResolver = ComposeContainerFactory(routineGuid);
            var @value = specifyResolver(userContext);
            return @value;
        }
    }
}