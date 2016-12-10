using System.Collections.Generic;

namespace Vse.AdminkaV1.Injected.Configuration
{
    public class LoggingPerformanceConfiguration : System.IProgress<string>
    {
        public string Category { get; private set; } = "performance";
        public decimal ThresholdSec { get; private set; } = 0.5M;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                Category = dictionary["Category"];
                ThresholdSec = decimal.Parse(dictionary["ThresholdSec"]);
            }
        }
    }
}
