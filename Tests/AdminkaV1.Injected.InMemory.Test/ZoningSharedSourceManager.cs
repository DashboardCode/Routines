using DashboardCode.Routines;
using System;

#if NETCOREAPP1_1 || NETCOREAPP2_0
using DashboardCode.AdminkaV1.Injected.NETStandard;
#else
using DashboardCode.AdminkaV1.Injected.NETFramework;
#endif

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public static class ZoningSharedSourceManager
    {
        public static IAdmikaConfigurationFacade GetConfiguration(string databaseName)
        {
            return new InMemoryAdmikaConfigurationFacade(databaseName);
        }
    }

    public class AdminkaInMemoryTestRoutine : AdminkaRoutineHandler
    {
        public AdminkaInMemoryTestRoutine(RoutineGuid routineGuid, object input, string name = "adminka")
            : this(routineGuid, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), input)
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag, object input, string name = "adminka")
            : this(memberTag, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), input)
        {
        }

        public AdminkaInMemoryTestRoutine(RoutineGuid routineGuid,  string name = "adminka")
            : this(routineGuid, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), new { })
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag,  string name = "adminka")
            : this(memberTag, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), new { })
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag, UserContext userContext, IAdmikaConfigurationFacade admikaConfigurationFacade, object input )
           : base(memberTag, userContext, admikaConfigurationFacade, input)
        {
        }

        public AdminkaInMemoryTestRoutine(RoutineGuid routineGuid, UserContext userContext, IAdmikaConfigurationFacade admikaConfigurationFacade, object input)
           : base(routineGuid, userContext, admikaConfigurationFacade, input)
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
