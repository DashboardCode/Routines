#if NETCOREAPP1_1
    using DashboardCode.AdminkaV1.Injected.NETStandard.Test;
#else
    using DashboardCode.AdminkaV1.Injected.NETFramework.Test;
#endif 

namespace DashboardCode.AdminkaV1.Injected.Test
{
    public static class ZoneManager
    {
        public static IAppConfiguration GetConfiguration()
        {

#if NETCOREAPP1_1
            return new ConfigurationNETStandard();
#else
            return new ConfigurationNETFramework();
#endif
        }
    }
}
