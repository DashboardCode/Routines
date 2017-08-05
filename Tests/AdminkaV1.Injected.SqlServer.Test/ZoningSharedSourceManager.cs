#if NETCOREAPP1_1
    using DashboardCode.AdminkaV1.Injected.SqlServer.NETCore.Test;
#else
    using DashboardCode.AdminkaV1.Injected.SqlServer.NETFramework.Test;
#endif 


namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{

    public static class ZoningSharedSourceManager
    {
        public static IAppConfiguration GetConfiguration()
        {
#if NETCOREAPP1_1
            return new ConfigurationNETCore(StorageType.SQLSERVER);
#else
            return new ConfigurationNETFramework(StorageType.SQLSERVER);
#endif
        }
    }
}
