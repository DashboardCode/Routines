using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.Test
{
    public class LoggingThresholdConfiguration : IProgress<Dictionary<string, string>>
    {
        public decimal ThresholdSec { get; internal set; } = 0;

        public void Report(Dictionary<string, string> section)
        {
            if (section != null)
            {
                //var t = StaticTools.DeserializeJson<Dictionary<string, string>>(json);
                ThresholdSec = decimal.Parse(section["ThresholdSec"]);
            }
        }
    }
}