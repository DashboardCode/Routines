namespace DashboardCode.Routines.Configuration.Test
{
    public class LoggingConfiguration
    {
        public bool StartActivity { get; set; }
        public bool FinishActivity { get; set; }
        public bool Input { get; set; }
        public bool Output { get; set; }
        public bool Verbose { get; set; }
        public bool ShouldBufferVerbose { get; set; }
        public bool ShouldVerboseWithStackTrace { get; set; }
    }
}
