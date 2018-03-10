using System;

namespace DashboardCode.Routines.Configuration
{
    public class ContainerFactory<TUserContext>
    {
        IConfigurationContainerFactory configurationContainerFactory;
        Func<TUserContext, string> getVerboseLoggingFlag;
        IGFactory<string> deserializer;
        public ContainerFactory(
            IConfigurationContainerFactory configurationContainerFactory,
            Func<TUserContext, string> getVerboseLoggingFlag,
            IGFactory<string> deserializer)
        {
            this.configurationContainerFactory = configurationContainerFactory;
            this.getVerboseLoggingFlag = getVerboseLoggingFlag;
            this.deserializer = deserializer;
        }

        public IContainer CreateContainer(MemberTag memberTag, TUserContext userContext)
        {
            var @for = getVerboseLoggingFlag(userContext);
            var configurationContainer = configurationContainerFactory
                .Create(memberTag, @for);
            
            return new Container(configurationContainer, deserializer);
        }
    }
}