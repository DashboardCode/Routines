using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkForEach
    {
        List<int> testData = new List<int>();
        int[] testArray;
        public BenchmarkForEach()
        {
            for(int i=0;i<10000;i++)
            {
                testData.Add(i);
            }
            testArray = testData.ToArray();
        }

        private List<int> GetList()
        {
            return testData;
        }

        private int[] GetArray()
        {
            return testArray;
        }

        [Benchmark]
        public long TestListCacheTmpEnumerable()
        {
            var x = 0;
            IEnumerable<int> tmp = GetList();
            foreach (var i in tmp)
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestListCacheTmp()
        {
            var x = 0;
            var tmp = GetList();
            foreach (var i in tmp)
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestListInCall()
        {
            var x = 0;
            foreach (var i in GetList())
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestList()
        {
            var x = 0;
            foreach (var i in testData)
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestArrayCacheTmpEnumerable()
        {
            var x = 0;
            IEnumerable<int> tmp = GetArray();
            foreach (var i in tmp)
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestArrayCacheTmp()
        {
            var x = 0;
            var tmp = GetArray();
            foreach (var i in tmp)
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestArrayInCall()
        {
            var x = 0;
            foreach (var i in GetArray())
            {
                x += i;
            }
            return x;
        }

        [Benchmark]
        public long TestArray()
        {
            var x = 0;
            foreach (var i in testArray)
            {
                x += i;
            }
            return x;
        }
    }
}