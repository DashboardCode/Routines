using System;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.Routines.Injected;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public class AdminkaInMemoryTestRoutine : AdminkaRoutineHandler
    {
        public AdminkaInMemoryTestRoutine(List<string> logger, MemberTag memberTag, object input, string name = "adminka")
            : this(
                  InjectedManager.ComposeListMemberLogger(logger),
                  memberTag,
                  input)
        {
        }

        public AdminkaInMemoryTestRoutine(Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory, MemberTag memberTag, object input, string name = "adminka")
            : this(memberTag, new UserContext("UnitTest"), ZoningSharedSourceProjectManager.GetConfiguration(name), ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                  loggingTransientsFactory,
                  input)
        {
        }

        public AdminkaInMemoryTestRoutine(Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory, MemberTag memberTag, string name = "adminka")
            : this(memberTag, new UserContext("UnitTest"), ZoningSharedSourceProjectManager.GetConfiguration(name), ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                  loggingTransientsFactory,
                  new { })
        {
        }

        public AdminkaInMemoryTestRoutine(MemberTag memberTag, UserContext userContext,
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationContainerFactory configurationFactory,
            Func<Guid, MemberTag, (IMemberLogger, IAuthenticationLogging)> loggingTransientsFactory,
          object input)
           : base(adminkaStorageConfiguration, configurationFactory, loggingTransientsFactory, memberTag, userContext, input)
        {
        }
    }
}