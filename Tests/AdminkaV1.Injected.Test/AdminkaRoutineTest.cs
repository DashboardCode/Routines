using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.AdminkaV1.DomAuthentication;
using Vse.AdminkaV1.Injected.Configuration;
using System.Collections.Generic;

namespace Vse.AdminkaV1.Injected.Test
{
    [TestClass]
    public class AdminkaRoutineTest
    {
        [TestMethod]
        public void TouchRoutineRepository()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(AdminkaRoutineTest), nameof(TouchRoutineRepository), loggingTransientsFactory, new { });
            routine.HandleRepository<User>(repository =>
            {

            });
        }

        [TestMethod]
        public void TouchRoutineRepositoryException()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(AdminkaRoutineTest), nameof(TouchRoutineRepository), loggingTransientsFactory, new { });
            try
            {
                routine.HandleRepository<User>(repository =>
                {
                    var users = repository.ToList();
                    throw new ApplicationException("kuku");
                });
            }
            catch (ApplicationException ex)
            {
                if (logger.Count == 0)
                    throw new ApplicationException("no log entries?", ex);
            }
        }

        [TestMethod]
        public void TouchRoutine()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(AdminkaRoutineTest), nameof(TouchRoutine), loggingTransientsFactory, new { });
            routine.Handle(container =>
            {

            });
        }

        [TestMethod]
        public void TouchRoutineSystemUser()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTuple = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(nameof(Test), nameof(AdminkaRoutineTest), nameof(TouchRoutineSystemUser), userContext, loggingTuple, new { });
            routine.Handle(container =>
            {

            });
        }
    }
}
