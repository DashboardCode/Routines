﻿namespace DashboardCode.Routines.Configuration.Test
{
    public static class ZoningSharedSourceManager
    {
#if NETCOREAPP1_1
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
