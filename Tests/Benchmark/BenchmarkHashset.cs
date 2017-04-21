using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Vse.Routines;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkRoutines
    {
        [Benchmark]
        public void IncludesCopy()
        {

            var source = TestTools.CreateTestModel();
            var destination = new TestModel();
            var includes = TestTools.CreateInclude();
            MemberExpressionExtensions.Copy(source, destination, includes);
        }
        [Benchmark]
        public void IncludesEquals()
        {
            var source = TestTools.CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                            .ThenInclude(i => i.IndexName) // compare
                    .Include(i => i.ListTest);
            var b2 = MemberExpressionExtensions.Equals(source, source, includes);
        }

        [Benchmark]
        public void IncludesClone()
        {
            var source = TestTools.CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques);
            var destination = MemberExpressionExtensions.Clone(source, includes);
        }

        [Benchmark]
        public void Increment()
        {
            var i = 0; i++;
        }

        [Benchmark]
        public void Concantent()
        {
            var a = "asdasd1";
            var b = "asdasd2";
            var c = a + b;
        }
    }
}
