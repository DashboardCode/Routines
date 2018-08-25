namespace DashboardCode.Routines.Configuration.Test
{
    public class WrappedContainer
    {
        ConfigurationContainerTest configurationContainer;
        Deserializer serializer = new Deserializer();
        public WrappedContainer(string type, string member, string @for=null)
        {
            var loader = ZoningSharedSourceProjectManager.GetLoader();
            if (string.IsNullOrWhiteSpace(@for))
                configurationContainer = new ConfigurationContainerTest(loader.GetGetRoutineConfigurationRecords(), serializer, new MemberTag(type, member));
            else
                configurationContainer = new ConfigurationContainerTest(loader.GetGetRoutineConfigurationRecords(), serializer, new MemberTag(type, member), @for);
        }

        public T Resolve<T>() where T: new()
        {
            var t = configurationContainer.Resolve<T>();
            return t;
        }
    }
}