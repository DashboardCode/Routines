using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingConfiguration : System.IProgress<string>
    {
        public bool StartActivity  { get; set; } = false;
        public bool FinishActivity { get; set; } = true;
        public bool Input   { get; set; } = true;
        public bool Output  { get; set; } = false;
        public bool Verbose { get; set; } = false;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                if (dictionary.TryGetValue("StartActivity", out string startActivity ))
                    StartActivity = bool.Parse(startActivity);
                if (dictionary.TryGetValue("FinishActivity", out string finishActivity))
                    FinishActivity = bool.Parse(finishActivity);
                if (dictionary.TryGetValue("Input", out string input))
                    Input = bool.Parse(input);
                if (dictionary.TryGetValue("Output", out string output))
                    Output = bool.Parse(output);
                if (dictionary.TryGetValue("Verbose", out string verbose))
                    Verbose = bool.Parse(verbose);
            }
        }
    }
}