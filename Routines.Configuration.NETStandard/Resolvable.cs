namespace Vse.Routines.Configuration.NETStandard
{
    public class Resolvable : IResolvable
    {
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
