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
    public class BenchmarkAdminkaRoutineNLogLogger
    {

        static BenchmarkAdminkaRoutineNLogLogger()
        {
            //TestIsland.Clear(); // main reason is to cache ef core db context
        }

        [Benchmark]
        public void MeasureRoutineNLog()
        {
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLogger();
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineNLog), new { });
            routine.Handle(container =>
            {

            });
        }

        [Benchmark]
        public void MeasureRoutineNoAuthorizationNLog()
        {
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLogger();
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineNoAuthorizationNLog)), userContext, new { });
            routine.Handle(container =>
            {

            });
        }
        /// <summary>
        /// Measure speed of empty routine
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryNLog()
        {
            //var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLogger();
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryNLog), new { });
            IReadOnlyCollection<ParentRecord> parentRecords;
            routine.HandleRepository<ParentRecord>((repository, closure) =>
            {
                parentRecords = repository.List();
                closure.Verbose?.Invoke("sample");
            });
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryExceptionNLog()
        {
            //var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLogger();
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryExceptionNLog), new { });
            try
            {
                IReadOnlyCollection<ParentRecord> parentRecords;
                routine.HandleRepository<ParentRecord>((repository, closure) =>
                {
                    parentRecords = repository.List();
                    closure.Verbose?.Invoke("sample");
                    throw new Exception("Test exception");
                });
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Test exception"))
                    throw new Exception("Not expected exception", ex);
            }
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryExceptionMailNLog()
        {
            //var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLogger();
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryExceptionMailNLog), new { });
            try
            {
                IReadOnlyCollection<ParentRecord> parentRecords;
                routine.HandleRepository<ParentRecord>((repository, closure) =>
                {
                    parentRecords = repository.List();
                    closure.Verbose?.Invoke("sample");
                    throw new Exception("Test exception");
                });
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Test exception"))
                    throw new Exception("Not expected exception", ex);
            }
        }

        [Benchmark]
        public void MeasureRoutineRepositoryErrorNLog()
        {
            //var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLogger();
            var routine = new AdminkaRoutineHandler(
                ZoningSharedSourceProjectManager.GetConfiguration(),
                ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryErrorNLog), new { });
            IReadOnlyCollection<ParentRecord> parentRecords=
                routine.HandleRepository< IReadOnlyCollection<ParentRecord> , ParentRecord >((repository, closure) =>
                {
                    var output = repository.List();
                    closure.Verbose?.Invoke("sample");
                    return output;
                });
        }
    }
}