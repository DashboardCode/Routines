using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.TestDom;
using BenchmarkDotNet.Attributes.Exporters;

namespace BenchmarkAdminka
{

    [Config(typeof(MultipleRuntimesManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [MemoryDiagnoser]
    //[ClrJob(isBaseline: true), CoreJob]
    [HtmlExporter, MarkdownExporter]
    public class BenchmarkAdminkaRoutineListLogger
    {

        static BenchmarkAdminkaRoutineListLogger()
        {
            //TestIsland.Clear(); // main reason is to cache ef core db context
        }

        [Benchmark]
        public void MeasureRoutineLogList()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLogger(logger);
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineLogList), new { });
            routine.Handle(container =>
            {

            });
        }

        [Benchmark]
        public void MeasureRoutineNoAuthorizationLogList()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLogger(logger);
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineNoAuthorizationLogList)), userContext, new { });
            routine.Handle(container =>
            {

            });
        }
        /// <summary>
        /// Measure speed of empty routine
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryLogList()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLogger(logger);
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineRepositoryLogList), new { });
            routine.HandleRepository<ParentRecord>(repository =>
            {
                var parentRecords = repository.List();
            });
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryExceptionLogList()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            //TODO: pass loggingConfiguration
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLogger(logger/*, loggingConfiguration*/);

            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineRepositoryExceptionLogList), new { });
            try
            {
                routine.HandleRepository<ParentRecord>(repository =>
                {
                    var parentRecords = repository.List();
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