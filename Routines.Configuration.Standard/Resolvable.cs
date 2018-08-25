using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Configuration.Standard
{
    public class Resolvable : IResolvableConfigurationRecord<IConfigurationSection>
    {
        public string Namespace { get; set; }
        public string Type      { get; set; }
        public IConfigurationSection Value { get; set; }
    }
}
