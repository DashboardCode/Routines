using DashboardCode.Routines;
using System;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Logging;
using System.Collections.Generic;

#if NETCOREAPP1_1 || NETCOREAPP2_0
using DashboardCode.AdminkaV1.Injected.NETStandard;
#else
using DashboardCode.AdminkaV1.Injected.NETFramework;
#endif

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public static class ZoningSharedSourceManager
    {
        public static AdminkaStorageConfiguration GetConfiguration(string databaseName)
        {
            return new InMemoryAdmikaConfigurationFacade(databaseName).ResolveAdminkaStorageConfiguration();
        }

        public static IConfigurationFactory GetConfigurationFactory()
        {
            return new ConfigurationFactory();
        }
        // configurationFactory,
    }

    public class AdminkaInMemoryTestRoutine : AdminkaRoutineHandler
    {
        public AdminkaInMemoryTestRoutine(List<string> logger, MemberTag memberTag, object input, string name = "adminka")
            : this(
                  InjectedManager.ComposeListLoggingTransients(logger),
                  memberTag,
                  input)
        {
        }

        public AdminkaInMemoryTestRoutine(Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory, MemberTag memberTag, object input, string name = "adminka")
            : this(memberTag, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), ZoningSharedSourceManager.GetConfigurationFactory(),
                  loggingTransientsFactory,
                  input)
        {
        }

        public AdminkaInMemoryTestRoutine(Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory, MemberTag memberTag,  string name = "adminka")
            : this(memberTag, new UserContext("UnitTest"), ZoningSharedSourceManager.GetConfiguration(name), ZoningSharedSourceManager.GetConfigurationFactory(),
                  loggingTransientsFactory,
                  new { })
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag, UserContext userContext,
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationFactory configurationFactory,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingTransientsFactory,
          object input )
           : base(adminkaStorageConfiguration, configurationFactory, loggingTransientsFactory, memberTag, userContext, input)
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
