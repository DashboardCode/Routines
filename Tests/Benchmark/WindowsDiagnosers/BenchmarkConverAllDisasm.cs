using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    //[Config(typeof(ManualWindowsDiagnosersConfig))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
#if !(NETCOREAPP1_1  || NETCOREAPP2_0 || NETCOREAPP2_1)
    //[HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.BranchMispredictions, BenchmarkDotNet.Diagnosers.HardwareCounter.BranchInstructions)]
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    [BenchmarkDotNet.Attributes.Jobs.RyuJitX64Job]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.InliningDiagnoser]
#endif
    public class BenchmarkConverAllDisasm
    {
        IReadOnlyCollection<string[]> testCol;
        List<string[]> testArray;
        public BenchmarkConverAllDisasm()
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