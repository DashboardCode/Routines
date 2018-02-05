using System;

namespace DashboardCode.Routines.Configuration
{
    public class ContainerFactory<TUserContext>
    {
        IConfigurationFactory configurationFactory;
        Func<TUserContext, string> getVerboseLoggingFlag;
        IGFactory<string> deserializer;
        public ContainerFactory(
            IConfigurationFactory configurationFactory,
            Func<TUserContext, string> getVerboseLoggingFlag,
            IGFactory<string> deserializer)
        {
            this.configurationFactory = configurationFactory;
            this.getVerboseLoggingFlag = getVerboseLoggingFlag;
            this.deserializer = deserializer;
        }

        public IContainer CreateContainer(RoutineGuid routineGuid, TUserContext userContext)
        {
            var @for = getVerboseLoggingFlag(userContext);
            var configurationContainer = configurationFactory.ComposeSpecify(routineGuid.MemberTag, @for);
            return new Container(configurationContainer, deserializer);
        }
    }
}