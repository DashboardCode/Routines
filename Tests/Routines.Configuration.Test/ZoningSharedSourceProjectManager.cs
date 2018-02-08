namespace DashboardCode.Routines.Configuration.Test
{
    public static class ZoningSharedSourceProjectManager
    {
#if NETCOREAPP2_0
        public static IConfigurationManagerLoader GetLoader()
        {
            var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            Microsoft.Extensions.Configuration.JsonConfigurationExtensions.AddJsonFile(configurationBuilder, "appsettings.json", false, true);
	        var configurationRoot = configurationBuilder.Build();
            return new NETStandard.ConfigurationManagerLoader(configurationRoot);
        }
#else
        public static IConfigurationManagerLoader GetLoader()
        {
            return new NETFramework.ConfigurationManagerLoader();
        }
#endif
    }
}