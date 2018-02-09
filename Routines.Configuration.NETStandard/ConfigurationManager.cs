using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    internal static class ConfigurationManager
    {
        internal static IConfigurationRoot ResolveConfigurationRoot()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            return configurationRoot;
        }
    }
}