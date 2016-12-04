using System;
using Vse.Routines;
using Vse.Routines.Configuration;

namespace Vse.AdminkaV1.Injected
{
    public class Resolver : IResolver
    {
        IConfigurationContainer configurationContainer;
        public Resolver(IConfigurationContainer configurationContainer)
        {
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
                    t = IoCManager.DeserializeJson<T>(serialized);
                }
            }
            return t;
        }
    }
}
