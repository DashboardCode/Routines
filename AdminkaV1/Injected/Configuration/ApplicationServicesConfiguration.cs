using System;
using System.Collections.Generic;

namespace Vse.AdminkaV1.Injected.Configuration
{
    public class ApplicationServicesConfiguration : IProgress<string>
    {
        public string InstanceName { get; private set; }

        public void Report(string json)
        {
            var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
            InstanceName = dictionary["InstanceName"];
        }
    }
}
