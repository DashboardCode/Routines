﻿namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    public static class ZoningSharedSourceManager
    {
        public static IApplicationFactory GetConfiguration()
        {
#if NETCOREAPP1_1 || NETCOREAPP2_0
            return new NETCore.Test.ApplicationFactory();
#else
            return new NETFramework.Test.ApplicationFactory();
#endif
        }
    }
}
