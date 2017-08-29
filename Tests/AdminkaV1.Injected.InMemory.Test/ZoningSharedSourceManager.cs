using DashboardCode.Routines;
using System;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public static class ZoningSharedSourceManager
    {
        public static IApplicationFactory GetConfiguration(string databaseName)
        {
#if NETCOREAPP1_1 || NETCOREAPP2_0
            return new NETCore.Test.ApplicationFactory(databaseName);
#else
            return new NETFramework.Test.ApplicationFactory(databaseName);
#endif
        }
    }

    public class AdminkaInMemoryTestRoutine : AdminkaRoutineHandler
    {
        public AdminkaInMemoryTestRoutine(RoutineGuid routineGuid, object input, string name = "adminka")
            : base(routineGuid, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), input)
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag, object input, string name = "adminka")
            : base(memberTag, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), input)
        {
        }

        public AdminkaInMemoryTestRoutine(RoutineGuid routineGuid,  string name = "adminka")
            : base(routineGuid, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), new { })
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag,  string name = "adminka")
            : base(memberTag, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), new { })
        {
        }
    }

    public abstract class RepositoryBaseTest
    {
        public static readonly string readonlyDatabaseName = "adminka_readonly_" + Guid.NewGuid();
        static RepositoryBaseTest()
        {
            TestIsland.Reset(readonlyDatabaseName);
        }
    }

}
