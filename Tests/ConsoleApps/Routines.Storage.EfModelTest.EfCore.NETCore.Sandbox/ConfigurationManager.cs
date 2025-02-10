using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore.Sandbox
{
    public static class ConfigurationManager
    {
        public static IConfigurationRoot ResolveConfigurationRoot()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            var configurationRoot = configurationBuilder.Build();
            return configurationRoot;
        }
    }
}