using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.AdminkaV1.TestDom;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class RoutineMeasureTest
    {
        public RoutineMeasureTest()
        {
            TestIsland.Clear(); // main reason is to cache ef core db context
        }

        [TestMethod]
        public void MeasureRoutine()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var admikaConfigurationFacade = ZoningSharedSourceManager.GetConfiguration();
            var routine = new AdminkaRoutineHandler(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutine), loggingTransientsFactory,   admikaConfigurationFacade,   new { });
            routine.Handle(container =>
            {

            });
        }

        [TestMethod]
        public void MeasureRoutineNoAuthorization()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTuple = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var userContext = new UserContext("UnitTest");
            var admikaConfigurationFacade = ZoningSharedSourceManager.GetConfiguration();
            var routine = new AdminkaRoutineHandler(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineNoAuthorization), userContext, loggingTuple, admikaConfigurationFacade, new { });
            routine.Handle(container =>
            {

            });
        }
        /// <summary>
        /// Measure speed of empty routine
        /// </summary>
        [TestMethod]
        public void MeasureRoutineRepository()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var admikaConfigurationFacade = ZoningSharedSourceManager.GetConfiguration();
            var routine = new AdminkaRoutineHandler(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), loggingTransientsFactory, admikaConfigurationFacade,   new { });
            routine.HandleRepository<ParentRecord>(repository =>
            {

            });
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
        [TestMethod]
        public void MeasureRoutineRepositoryException()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var admikaConfigurationFacade = ZoningSharedSourceManager.GetConfiguration();
            var routine = new AdminkaRoutineHandler(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), loggingTransientsFactory,  admikaConfigurationFacade,  new { });
            try
            {
                routine.HandleRepository<ParentRecord>(repository =>
                {
                    var users = repository.List();
                    throw new Exception("Test exception");
                });
            }
            catch (Exception ex)
            {
                if (logger.Count == 0)
                    throw new Exception("no log entries?", ex);
            }
        }
    }
}
