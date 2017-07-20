#if NETCOREAPP1_1
    using Xunit;
    using Vse.AdminkaV1.Injected.NETStandard.Test;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Vse.AdminkaV1.Injected.NETFramework.Test;
#endif
using System.Collections.Generic;
using Vse.AdminkaV1.Injected.Configuration;
using Vse.AdminkaV1.DomTest;
using System;

namespace Vse.AdminkaV1.Injected.Test
{
#if !NETCOREAPP1_1
    [TestClass]
#endif
    public class RoutineMeasureTest
    {
#if NETCOREAPP1_1
        ConfigurationNETStandard Configuration = new ConfigurationNETStandard();
#else
        ConfigurationNETFramework Configuration = new ConfigurationNETFramework();
#endif

        public RoutineMeasureTest()
        {
            TestIsland.Clear(); // main reason is to cache ef core db context
        }

#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public void MeasureRoutine()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutine), loggingTransientsFactory, Configuration, new { });
            routine.Handle(container =>
            {

            });
        }

#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public void MeasureRoutineNoAuthorization()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTuple = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineNoAuthorization), userContext, loggingTuple, Configuration, new { });
            routine.Handle(container =>
            {

            });
        }
        /// <summary>
        /// Measure speed of empty routine
        /// </summary>
#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public void MeasureRoutineRepository()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), loggingTransientsFactory, Configuration,  new { });
            routine.HandleRepository<ParentRecord>(repository =>
            {

            });
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
#if NETCOREAPP1_1
        [Fact]
#else
        [TestMethod]
#endif
        public void MeasureRoutineRepositoryException()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingVerboseConfiguration = new LoggingVerboseConfiguration();
            var loggingPerformanceConfiguration = new LoggingPerformanceConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration,
                loggingVerboseConfiguration, loggingPerformanceConfiguration);
            var routine = new AdminkaRoutine(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), loggingTransientsFactory, Configuration,  new { });
            try
            {
                routine.HandleRepository<ParentRecord>(repository =>
                {
                    var users = repository.ToList();
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
