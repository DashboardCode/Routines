using System;

namespace DashboardCode.Routines.Configuration
{
    public class ContainerFactory<TUserContext> 
    {
        IConfigurationContainerFactory configurationContainerFactory;
        Func<TUserContext, string> getVerboseLoggingFlag;
        public ContainerFactory(
            IConfigurationContainerFactory configurationContainerFactory,
            Func<TUserContext, string> getVerboseLoggingFlag
            )
        {
            this.configurationContainerFactory = configurationContainerFactory;
            this.getVerboseLoggingFlag = getVerboseLoggingFlag;
        }

        public IContainer CreateContainer(MemberTag memberTag, TUserContext userContext)
        {
            var @for = getVerboseLoggingFlag(userContext);
            var configurationContainer = configurationContainerFactory
                .Create(memberTag, @for);
            
            return configurationContainer;
        }
    }
}