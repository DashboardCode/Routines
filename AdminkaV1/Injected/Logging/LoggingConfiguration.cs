using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingConfiguration : System.IProgress<string>
    {
        public bool StartActivity   { get; private set; } = false;
        public bool FinishActivity  { get; private set; } = true;
        public decimal ThresholdSec { get; private set; } = 0; //0.5M;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                if (dictionary.TryGetValue("StartActivity", out string startActivity ))
                    StartActivity = bool.Parse(startActivity);
                if (dictionary.TryGetValue("FinishActivity", out string finishActivity))
                    FinishActivity = bool.Parse(finishActivity);
                if (dictionary.TryGetValue("ThresholdSec", out string thresholdSec))
                    ThresholdSec = decimal.Parse(thresholdSec);
            }
        }
    }
}