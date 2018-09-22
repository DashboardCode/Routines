using System;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Injected.Logging;

using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.Routines.Injected.Logging;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public class AdminkaInMemoryTestRoutine : AdminkaRoutineHandler
    {
        public AdminkaInMemoryTestRoutine(
            List<string> logger, 
            MemberTag memberTag, 
            object input, 
            string name = "adminka")
            : this(
                  InjectedManager.ComposeListMemberLoggerFactory(logger),
                  memberTag,
                  input,
                  name)
        {
        }

        private AdminkaInMemoryTestRoutine(
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
            MemberTag memberTag,
            object input,
            string name)
            : base( 
                 new AdminkaStorageConfiguration(name, null, StorageType.INMEMORY),
                 TestManager.ApplicationSettings.PerformanceCounters,
                 TestManager.ApplicationSettings.ConfigurationContainerFactory,
                 loggingTransientsFactory, 
                 memberTag,
                 new UserContext("UnitTest"), 
                 input)
        {
        }
    }
}