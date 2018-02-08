using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

using DashboardCode.AdminkaV1;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.Routines;

namespace BenchmarkAdminka
{
    //[RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob(isBaseline: true), CoreJob]
    //[HtmlExporter, MarkdownExporter]
    //[MemoryDiagnoser]
    [RankColumn]
    public class BenchmarkAdminkaRoutine
    {
        
        static BenchmarkAdminkaRoutine()
        {
            //TestIsland.Clear(); // main reason is to cache ef core db context
        }

        [Benchmark]
        public void MeasureRoutineLogList()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutine), nameof(MeasureRoutineLogList), new { });
            routine.Handle(container =>
            {

            });
        }

        [Benchmark]
        public void MeasureRoutineNoAuthorizationLogList()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutine), nameof(MeasureRoutineNoAuthorizationLogList)), userContext, new { });
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
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutine), nameof(MeasureRoutineRepositoryLogList), new { });
            routine.HandleRepository<ParentRecord>(repository =>
            {

            });
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryExceptionLogList()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger, loggingConfiguration);

            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutine), nameof(MeasureRoutineRepositoryExceptionLogList), new { });
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