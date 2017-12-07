using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Configuration
{
    public class AdConfiguration : System.IProgress<string>
    {
        public bool UseAdAuthorization { get; private set; } = true;
        public void Report(string json)
        {
            var dictionary = InjectedManager.DeserializeJson <Dictionary<string, string>>(json);
            UseAdAuthorization = bool.Parse(dictionary["UseAdAuthorization"]);
        }
    }
}