namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationContainerFactory
    {
        IContainer Create(MemberTag memberTag, string @for);
    }

    public class ConfigurationContainerFactory<TSerialized>: IConfigurationContainerFactory
    {
        readonly IConfigurationManagerLoader<TSerialized> configurationManagerLoader;
        readonly IGWithConstructorFactory<TSerialized> deserializer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationManagerLoader">Abstraction over .NET Classic and .NET Core configuration methods</param>
        public ConfigurationContainerFactory(IConfigurationManagerLoader<TSerialized> configurationManagerLoader, IGWithConstructorFactory<TSerialized> deserializer)
        {
            this.configurationManagerLoader = configurationManagerLoader;
            this.deserializer = deserializer;
        }

        public IContainer Create(MemberTag memberTag, string @for) {
            var routineConfigurationRecords = configurationManagerLoader.GetGetRoutineConfigurationRecords();
            return new ConfigurationContainer<TSerialized>(routineConfigurationRecords, deserializer, memberTag, @for);
        }
    }
}