using System;

namespace DashboardCode.Routines.Configuration.Test
{
    public class State
    {
        IConfigurationContainer configContainer;
        public State(string controller, string action, string @for=null)
        {
            var basicConfigContainer = ZoneManager.GetConfiguration().GetSpecifiableConfigurationContainer(null, controller, action);
            if (string.IsNullOrWhiteSpace(@for))
            {
                configContainer = basicConfigContainer;
            }else
            {
                configContainer = basicConfigContainer.Specify(@for);
            }
        }

        public T Resolve<T>() where T: new()
        {
            var t = new T();
            var serialized = configContainer.ResolveSerialized<T>();
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
