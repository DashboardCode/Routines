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
            T t = new T();
            var serialized = configurationContainer.ResolveString<T>();
            if (serialized != null)
            {
                if (t is ISetter<string>)
                    ((ISetter<string>)t).Set(serialized);
                else if (t is IProgress<string>)
                    ((IProgress<string>)t).Report(serialized);
                else
                    t = deserializer.Create<T>(serialized);
            }
            return t;
        }
    }
}