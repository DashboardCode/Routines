using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingVerboseConfiguration :IProgress<string>
    {
        public bool Verbose { get; private set; } = false;
        public string FlashBufferRuleLang        { get; private set; } = null;
        public string FlashBufferRule            { get; private set; } = null;
        public bool ShouldVerboseWithStackTrace { get; private set; } = false;
        public bool Input   { get; private set; } = true;
        public bool Output  { get; private set; } = false;
        

        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                if (dictionary.TryGetValue("ShouldVerboseWithStackTrace", out string verboseWithStackTrace))
                    ShouldVerboseWithStackTrace = bool.Parse(verboseWithStackTrace);

                if (dictionary.TryGetValue("FlashBufferRuleLang", out string flashBufferRuleLang))
                    FlashBufferRuleLang = flashBufferRuleLang;
                if (dictionary.TryGetValue("FlashBufferRule", out string flashBufferRule))
                    FlashBufferRule = flashBufferRule;

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