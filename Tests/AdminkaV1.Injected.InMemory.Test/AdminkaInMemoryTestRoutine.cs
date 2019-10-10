using System;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Logging;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public class AdminkaInMemoryTestRoutine : AdminkaAnonymousRoutineHandler
    {
        public AdminkaInMemoryTestRoutine(
            List<string> logger, 
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag, 
            object input, 
            string name = "adminka")
            : this(
                  InjectedManager.ComposeListMemberLoggerFactory(logger),
                  hasVerboseLoggingPrivilege,
                  memberTag,
                  input,
                  name)
        {
        }

        private AdminkaInMemoryTestRoutine(
            Func<Guid, MemberTag, IMemberLogger> loggingTransientsFactory,
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag,
            object input,
            string name)
            : base(
#if NETCOREAPP
                 InjectedManager.CreateInMemoryApplicationSettingsStandard(name),
#else
                 InjectedManager.CreateInMemoryApplicationSettingsClassic(name),
#endif
                 loggingTransientsFactory,
                 hasVerboseLoggingPrivilege: hasVerboseLoggingPrivilege,
                 memberTag,
                 "UnitTest", 
                 input)
        {
        }
    }
}