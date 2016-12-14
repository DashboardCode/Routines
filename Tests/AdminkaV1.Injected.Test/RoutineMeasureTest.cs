using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.AdminkaV1.Injected.Configuration;
using Vse.AdminkaV1.DomTest;

namespace Vse.AdminkaV1.Injected.Test
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
            var loggingTransientsFactory = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutine), loggingTransientsFactory, new { });
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
            var loggingTuple = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineNoAuthorization), userContext, loggingTuple, new { });
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
            var loggingTransientsFactory = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), loggingTransientsFactory, new { });
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
            var loggingTransientsFactory = InjectedManager.ListConstructor(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), loggingTransientsFactory, new { });
            try
            {
                routine.HandleRepository<ParentRecord>(repository =>
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
    }
}
