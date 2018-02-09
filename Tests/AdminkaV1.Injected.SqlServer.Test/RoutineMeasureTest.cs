// MOVED TO BenchmarkAdminka
// delete it with new BenchmarkAdminka version that will support multitargeting

//using System;
//using System.Collections.Generic;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using DashboardCode.Routines;
//using DashboardCode.AdminkaV1.TestDom;
//using DashboardCode.AdminkaV1.Injected.Logging;

//namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
//{
//    [TestClass]
//    public class RoutineMeasureTest
//    {
//        public RoutineMeasureTest() 
//        {
//            TestIsland.Clear(); // main reason is to cache ef core db context
//        }

//        [TestMethod]
//        public void MeasureRoutine()
//        {
//            var logger = new List<string>();
//            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);
//            var routine = new AdminkaRoutineHandler(
//                ZoningSharedSourceProjectManager.GetConfiguration(),
//                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
//                loggingTransientsFactory,
//                nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutine), new { });
//            routine.Handle(container =>
//            {

//            });
//        }

//        [TestMethod]
//        public void MeasureRoutineNoAuthorization()
//        {
//            var logger = new List<string>();
//            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);
//            var userContext = new UserContext("UnitTest");
//            var routine = new AdminkaRoutineHandler(
//                ZoningSharedSourceProjectManager.GetConfiguration(),
//                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
//                loggingTransientsFactory,
//                new MemberTag(nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineNoAuthorization)), userContext, new { });
//            routine.Handle(container =>
//            {

//            });
//        }
//        /// <summary>
//        /// Measure speed of empty routine
//        /// </summary>
//        [TestMethod]
//        public void MeasureRoutineRepository()
//        {
//            var logger = new List<string>();
//            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);
//            var routine = new AdminkaRoutineHandler(
//                ZoningSharedSourceProjectManager.GetConfiguration(),
//                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
//                loggingTransientsFactory,
//                nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository),  new { });
//            routine.HandleRepository<ParentRecord>(repository =>
//            {

//            });
//        }

//        /// <summary>
//        /// Measure speed of empty routine and exception handler
//        /// </summary>
//        [TestMethod]
//        public void MeasureRoutineRepositoryException()
//        {
//            var logger = new List<string>();
//            var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
//            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration);

//            var routine = new AdminkaRoutineHandler(
//                ZoningSharedSourceProjectManager.GetConfiguration(),
//                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
//                loggingTransientsFactory,
//                nameof(Test), nameof(RoutineMeasureTest), nameof(MeasureRoutineRepository), new { });
//            try
//            {
//                routine.HandleRepository<ParentRecord>(repository =>
//                {
//                    var users = repository.List();
//                    throw new Exception("Test exception");
//                });
//            }
//            catch (Exception ex)
//            {
//                if (logger.Count == 0)
//                    throw new Exception("no log entries?", ex);
//            }
//        }
//    }
//}
