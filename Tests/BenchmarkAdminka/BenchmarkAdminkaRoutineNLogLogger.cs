using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

using DashboardCode.Routines;

using DashboardCode.AdminkaV1;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.TestDom;

namespace BenchmarkAdminka
{
    [Config(typeof(MultipleRuntimesManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [MemoryDiagnoser]
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
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLoggerFactory(null);
            var routine = new AdminkaAnonymousRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineNLog)), 
                "Anonymous",new { });
            routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().Handle(repository =>
            {

            }));
        }

        [Benchmark]
        public void MeasureRoutineNoAuthorizationNLog()
        {
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLoggerFactory(null);
            var routine = new AdminkaAnonymousRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineNoAuthorizationNLog)), "UnitTest", new { });
            routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().Handle(repository =>
            {

            }));
        }
        /// <summary>
        /// Measure speed of empty routine
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryNLog()
        {
            //var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLoggerFactory(null);
            var routine = new AdminkaAnonymousRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryNLog)), 
                "Anonymous", new { });
            IReadOnlyCollection<ParentRecord> parentRecords;
            routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().HandleRepository<ParentRecord>(repository =>
            {
                parentRecords = repository.List();
                closure.Verbose?.Invoke("sample");
            }));
        }

        /// <summary>
        /// Measure speed of empty routine and exception handler
        /// </summary>
        [Benchmark]
        public void MeasureRoutineRepositoryExceptionNLog()
        {
            //var loggingConfiguration = new LoggingConfiguration() { Verbose = true };
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLoggerFactory(null);
            var routine = new AdminkaAnonymousRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryExceptionNLog)), 
                "Anonymous", new { });
            try
            {
                IReadOnlyCollection<ParentRecord> parentRecords;
                routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().HandleRepository<ParentRecord>(repository =>
                {
                    parentRecords = repository.List();
                    closure.Verbose?.Invoke("sample");
                    throw new Exception("Test exception");
                }));
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
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLoggerFactory(null);
            var routine = new AdminkaAnonymousRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryExceptionMailNLog)), 
                "Anonymous", new { });
            try
            {
                IReadOnlyCollection<ParentRecord> parentRecords;
                routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().HandleRepository<ParentRecord>(repository =>
                {
                    parentRecords = repository.List();
                    closure.Verbose?.Invoke("sample");
                    throw new Exception("Test exception");
                }));
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
            var loggingTransientsFactory = InjectedManager.ComposeNLogMemberLoggerFactory(null);
            var routine = new AdminkaAnonymousRoutineHandler(
                Program.ApplicationSettings,
                loggingTransientsFactory,
                new MemberTag("Test", nameof(BenchmarkAdminkaRoutineNLogLogger), nameof(MeasureRoutineRepositoryErrorNLog))
                , "Anonymous", 
                new { });
            IReadOnlyCollection<ParentRecord> parentRecords=
                routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().HandleRepository<IReadOnlyCollection<ParentRecord>, ParentRecord>(repository =>
                {
                    var output = repository.List();
                    closure.Verbose?.Invoke("sample");
                    return output;
                }));
        }
    }
}