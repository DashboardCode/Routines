using System;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Logging;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    public class AdminkaInMemoryTestRoutine : AdminkaAnonymousRoutineHandler
    {
        public readonly static ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsClassic();

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
            Func<Guid, MemberTag, IMemberLogger> loggingTransientsFactory,
            MemberTag memberTag,
            object input,
            string name)
            : base(
                 ApplicationSettings.CreateInMemoryApplicationSettings(),
                 loggingTransientsFactory, 
                 memberTag,
                 "UnitTest", 
                 input)
        {
        }
    }
}