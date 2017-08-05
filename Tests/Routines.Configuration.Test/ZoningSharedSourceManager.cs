#if NETCOREAPP1_1
    using DashboardCode.Routines.Configuration.NETCore.Test;
#else
    using DashboardCode.Routines.Configuration.NETFramework.Test;
#endif 
namespace DashboardCode.Routines.Configuration.Test
{
    public static class ZoningSharedSourceManager
    {
#if NETCOREAPP1_1
        public static ConfigurationNETStandard GetConfiguration()
        {

            ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
            return new ConfigurationNETStandard();
        }
#else
        public static ConfigurationNETFramework GetConfiguration()
        {
            ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
            return new ConfigurationNETFramework();
        }
#endif
    }
}
