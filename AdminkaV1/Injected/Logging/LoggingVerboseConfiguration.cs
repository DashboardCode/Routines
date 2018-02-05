using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingVerboseConfiguration :IProgress<string>
    {
        public bool ShouldBufferVerbose { get; private set; } = true;
        public bool ShouldVerboseWithStackTrace { get; private set; } = false;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                ShouldBufferVerbose = bool.Parse(dictionary["ShouldBufferVerbose"]);
                string verboseWithStackTrace;
                if (dictionary.TryGetValue("ShouldVerboseWithStackTrace", out verboseWithStackTrace))
                {
                    ShouldVerboseWithStackTrace = bool.Parse(verboseWithStackTrace);
                }
            }
        }
    }
}