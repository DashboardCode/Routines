namespace DashboardCode.Routines.Configuration.NETStandard
{
    public class Resolvable : IResolvableConfigurationRecord
    {
        public string Namespace { get; set; }
        public string Type      { get; set; }
        public string Value     { get; set; }
    }
}
