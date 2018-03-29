using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Diagnostics
{
    public class DiagnosticsConfiguration : System.IProgress<string>
    {
        public string  InstanceName { get; private set; }
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                InstanceName = dictionary["InstanceName"];
            }
        }
    }
}