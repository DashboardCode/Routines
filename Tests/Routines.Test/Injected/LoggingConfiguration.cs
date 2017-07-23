namespace DashboardCode.Routines.Injected.Test
{
    public class LoggingConfiguration 
    {
        public bool ActivityStart {get; set;} = true;

        public decimal ActivityThresholdMSec  {get; set; } = 0;

        public bool BufferVerbose { get; set; } = true;

        public bool VerboseWithStackTrace { get; set; } = true;
    }
}
