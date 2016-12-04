using System.Collections.Generic;

namespace Vse.AdminkaV1.Injected.Configuration
{
    public class AdConfiguration : System.IProgress<string>
    {
        public bool UseAdAuthorization { get; private set; } = true;
        public void Report(string json)
        {
            var dictionary = IoCManager.DeserializeJson <Dictionary<string, string>>(json);
            UseAdAuthorization = bool.Parse(dictionary["UseAdAuthorization"]);
        }
    }
}
