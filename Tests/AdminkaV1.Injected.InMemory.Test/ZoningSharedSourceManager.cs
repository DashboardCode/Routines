#if NETCOREAPP1_1
    using DashboardCode.AdminkaV1.Injected.InMemory.NETCore.Test;
#else
    using DashboardCode.AdminkaV1.Injected.InMemory.NETFramework.Test;
#endif 


namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{

    public static class ZoningSharedSourceManager
    {
        public static IAppConfiguration GetConfiguration(string databaseName)
        {
#if NETCOREAPP1_1
            return new ConfigurationNETCore(StorageType.INMEMORY, null, databaseName);
#else
            return new ConfigurationNETFramework(StorageType.INMEMORY,  databaseName);
#endif
        }
    }
}
