using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using System.Collections.Generic;

namespace Benchmark
{
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkForEach
    {
        List<string> testData = new List<string>();
        string[] testArray;
        public BenchmarkForEach()
        {
            for(int i=0;i<1000;i++)
            {
                testData.Add(i.ToString());
            }
            testArray = testData.ToArray();
        }

        [Benchmark]
        public int TestList()
        {
            var x = 0;
            foreach(var i in testData)
            {
                x += i.Length;
            }
            return x;
        }

        [Benchmark]
        public int TestArray()
        {
            var x = 0;
            foreach (var i in testArray)
            {
                x += i.Length;
            }
            return x;
        }
    }
}
