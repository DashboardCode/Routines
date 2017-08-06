namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public static class ZoningSharedSourceManager
    {
        public static IApplicationFactory GetConfiguration(string databaseName)
        {
#if NETCOREAPP1_1
            return new NETCore.Test.ApplicationFactory(databaseName);
#else
            return new NETFramework.Test.ApplicationFactory(databaseName);
#endif
        }
    }
}
