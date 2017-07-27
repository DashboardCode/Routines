using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.Test
{
    public class LoggingPerformanceConfiguration : IProgress<string>
    {
        public string Category { get; internal set; } = "performance";
        public decimal ThresholdSec { get; internal set; } = 0;
        public void Report(string serialized)
        {
            var t = StaticTools.DeserializeJson<Dictionary<string,string>>(serialized);
            Category = t["Category"];
            ThresholdSec = decimal.Parse(t["ThresholdSec"]);
        }
    }
}
