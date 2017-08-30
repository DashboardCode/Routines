using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace BenchmarkClassic
{
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [ClrJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser, InliningDiagnoser]
    public class BenchmarkConverAll
    {
        IReadOnlyCollection<string[]> testCol;
        List<string[]> testArray;
        public BenchmarkConverAll()
        {
            var testData = new List<string[]>();
            for (int i=0;i<1000;i++)
            {
                var testData2 = new List<string>();
                for(int j= 0; j < 10; j++)
                    testData2.Add(j.ToString());
                testData.Add(testData2.ToArray());
            }
            testCol = testData;
            testArray = testData;
        }
        [Benchmark]
        public IEnumerable<string> TestConverAll()
        {
            var x = testArray.ConvertAll(e => string.Join(",",e));
            return x;
        }
        [Benchmark]
        public IEnumerable<string> TestSelect()
        {
            var x = testArray.ConvertAll(e => string.Join(",", e));
            return x;
        }
    }
}