namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class LoggingVerboseConfiguration 
    {
        public bool Verbose { get; set; } = false;
        public string ErrorRuleLang        { get; set; } = null;
        public string ErrorRule            { get; set; } = null;
        public bool ShouldVerboseWithStackTrace { get; set; } = false;
        public bool Input   { get; set; } = true;
        public bool Output  { get; set; } = false;
    }
}