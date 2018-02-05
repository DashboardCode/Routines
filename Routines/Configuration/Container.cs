using System;

namespace DashboardCode.Routines.Configuration
{
    public class Container : IContainer
    {
        ConfigurationContainer configurationContainer;
        IGFactory<string> deserializer;
        public Container(ConfigurationContainer configurationContainer, IGFactory<string> deserializer)
        {
            this.deserializer = deserializer;
            this.configurationContainer = configurationContainer;
        }
        public T Resolve<T>() where T : new()
        {
            var t = new T();
            var serialized = configurationContainer.ResolveSerialized<T>();
            if (t is IProgress<string>)
            {
                ((IProgress<string>)t).Report(serialized);
            }
            else
            {
                if (serialized != null)
                {
                    t = deserializer.Create<T>(serialized);
                }
            }
            return t;
        }
    }
}