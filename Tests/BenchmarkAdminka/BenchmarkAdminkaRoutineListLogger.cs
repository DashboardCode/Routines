using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.TestDom;

namespace BenchmarkAdminka
{
    [Config(typeof(MultipleRuntimesManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [MemoryDiagnoser]
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
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);
            var routine = new AdminkaRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineLogList),
                new { });
            routine.UserRoutineHandler.Handle(container =>
            {

            });
        }

        [Benchmark]
        public void MeasureRoutineNoAuthorizationLogList()
        {
            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);
            var userContext = new UserContext("UnitTest");
            var routine = new AdminkaRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineNoAuthorizationLogList)), userContext, new { });
            routine.UserRoutineHandler.Handle(container =>
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
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);
            var routine = new AdminkaRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineRepositoryLogList), new { });
            routine.StorageRoutineHandler.HandleRepository<ParentRecord>((repository, closure) =>
            {
                var parentRecords = repository.List();
                closure.Verbose?.Invoke("sample");
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
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger/*, loggingConfiguration*/);

            var routine = new AdminkaRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineRepositoryExceptionLogList), new { });
            try
            {
                IReadOnlyCollection<ParentRecord> parentRecords;
                routine.StorageRoutineHandler.HandleRepository<ParentRecord>((repository, closure) =>
               {
                   parentRecords = repository.List();
                   closure.Verbose?.Invoke("sample");
                   throw new Exception("Test exception");
               });
            }
            catch (Exception ex)
            {
                if (logger.Count == 0)
                    throw new Exception("no log entries?", ex);
            }
        }

        [Benchmark]
        public void MeasureRoutineRepositoryErrorLogList()
        {
            var logger = new List<string>();
            var loggingConfiguration = new LoggingConfiguration();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                "Test", nameof(BenchmarkAdminkaRoutineListLogger), nameof(MeasureRoutineRepositoryErrorLogList), new { });
             IReadOnlyCollection<ParentRecord> parentRecords =
                 routine.StorageRoutineHandler.HandleRepository<IReadOnlyCollection<ParentRecord>, ParentRecord >((repository, closure) =>
                 {
                     var output = repository.List();
                     closure.Verbose?.Invoke("sample");
                     return output;
                 });
        }
    }
}