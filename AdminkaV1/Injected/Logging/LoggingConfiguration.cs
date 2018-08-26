using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingConfiguration 
    {
        public bool StartActivity   { get; set; } = false;
        public bool FinishActivity  { get; set; } = true;
        public decimal ThresholdSec { get; set; } = 0; //0.5M;
    }
}