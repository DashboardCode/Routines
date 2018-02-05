using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Performance
{
    public class LoggingPerformanceConfiguration : System.IProgress<string>
    {
        public string InstanceName { get; private set; }
        public string Category { get; private set; } = "performance";
        public decimal ThresholdSec { get; private set; } = 0.5M;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                Category = dictionary["Category"];
                ThresholdSec = decimal.Parse(dictionary["ThresholdSec"]);
                InstanceName = dictionary["InstanceName"];
            }
        }
    }
}