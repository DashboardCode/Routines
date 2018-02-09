using System;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public abstract class RepositoryTestBase
    {
        public static readonly string readonlyDatabaseName = "adminka_readonly_" + Guid.NewGuid();
        static RepositoryTestBase() =>
            TestIsland.Reset(readonlyDatabaseName);
    }
}