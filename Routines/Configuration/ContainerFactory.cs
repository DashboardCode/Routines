using System;

namespace DashboardCode.Routines.Configuration
{
    public class ContainerFactory
    {
        IConfigurationContainerFactory configurationContainerFactory;
        public ContainerFactory(
            IConfigurationContainerFactory configurationContainerFactory
            )
        {
            this.configurationContainerFactory = configurationContainerFactory;
        }

        public IContainer CreateContainer(MemberTag memberTag, string @for)
        {
            var configurationContainer = configurationContainerFactory
                .Create(memberTag, @for);
            
            return configurationContainer;
        }
    }
 
}