using System;
using System.Text;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using DashboardCode.Routines.Json;

namespace Benchmark
{
    [Config(typeof(MyManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkActivatorCheck
    {
        [Benchmark]
        public TestClass CheckConstructor()
        {
            return new TestClass();
        }

        [Benchmark]
        public TestClass CheckActivator()
        {
            return (TestClass)Activator.CreateInstance(typeof(TestClass));
        }

        public class TestClass
        {
            public int IntValue {get;set; }
            public string TextValue { get; set; }
        }
    }
}
