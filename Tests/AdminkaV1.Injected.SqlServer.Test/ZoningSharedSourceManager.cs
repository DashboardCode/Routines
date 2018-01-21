#if NETCOREAPP1_1 || NETCOREAPP2_0
    using DashboardCode.AdminkaV1.Injected.NETStandard;
#else
    using DashboardCode.AdminkaV1.Injected.NETFramework;
#endif

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    public static class ZoningSharedSourceManager
    {
        public static IAdmikaConfigurationFacade GetConfiguration()
        {
            return new SqlServerAdmikaConfigurationFacade();
        }
    }
}
