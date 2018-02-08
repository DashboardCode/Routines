using System;

namespace DashboardCode.Routines.Configuration.Test
{
    public class WrappedContainer
    {
        ConfigurationContainer configurationContainer;
        public WrappedContainer(string type, string member, string @for=null)
        {
            var loader = ZoningSharedSourceProjectManager.GetLoader();
            if (string.IsNullOrWhiteSpace(@for))
                configurationContainer = new ConfigurationContainer(loader, new MemberTag(type, member));
            else
                configurationContainer = new ConfigurationContainer(loader, new MemberTag(type, member), @for);
        }

        public T Resolve<T>() where T: new()
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
                    t = StaticTools.DeserializeJson<T>(serialized);
                }
            }
            return t;
        }
    }
}