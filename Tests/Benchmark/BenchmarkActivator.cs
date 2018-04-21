using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
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