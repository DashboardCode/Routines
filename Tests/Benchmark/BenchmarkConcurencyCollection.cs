using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkConcurencyCollection
    {
        static IEnumerable<int> range;
        static BenchmarkConcurencyCollection()
        {
            range = Enumerable.Range(1, 10000);
        }

        [Benchmark]
        public void ConcurrentBagAdd()
        {
            var bag = new ConcurrentBag<int>();
            Parallel.For(0, 1000, i =>
            {
                bag.Add(i);
            });
        }

        [Benchmark]
        public void ListAdd()
        {
            var lst = new List<int>();
            Parallel.For(0, 1000, i =>
            {
                lock ("ListAdd")
                {
                    lst.Add(i);
                }
            });
        }

        [Benchmark]
        public void FilterLinq()
        {
            var filtered = range.Where(e => e % 2 == 0).ToList();
        }

        [Benchmark]
        public void FilterPLinq()
        {
            var filtered = range.AsParallel().Where(e => e % 2 == 0).ToList();
        }
    }
}