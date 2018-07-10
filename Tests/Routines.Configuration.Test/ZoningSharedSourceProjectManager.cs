namespace DashboardCode.Routines.Configuration.Test
{
    public static class ZoningSharedSourceProjectManager
    {
#if NETCOREAPP
        public static IConfigurationManagerLoader GetLoader()
        {
            var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            Microsoft.Extensions.Configuration.JsonConfigurationExtensions.AddJsonFile(configurationBuilder, "appsettings.json", false, true);
	        var configurationRoot = configurationBuilder.Build();
            return new Standard.ConfigurationManagerLoader(configurationRoot);
        }
#else
        public static IConfigurationManagerLoader GetLoader()
        {
            return new Classic.ConfigurationManagerLoader();
        }
#endif
    }
}