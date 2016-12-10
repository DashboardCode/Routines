using System.Collections.Generic;

namespace Vse.AdminkaV1.Injected.Configuration
{
    public class LoggingConfiguration : System.IProgress<string>
    {
        public bool StartActivity { get; set; } = false;
        public bool FinishActivity { get; set; } = true;
        public bool Input { get; set; } = true;
        public bool Output { get; set; } = false;
        public bool Verbose { get; set; } = false;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                StartActivity = bool.Parse(dictionary["StartActivity"]);
                FinishActivity = bool.Parse(dictionary["FinishActivity"]);
                Input = bool.Parse(dictionary["Input"]);
                Output = bool.Parse(dictionary["Output"]);
                Verbose = bool.Parse(dictionary["Verbose"]);
            }
        }
    }
}
