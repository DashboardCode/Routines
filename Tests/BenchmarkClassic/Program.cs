using BenchmarkDotNet.Running;
using System;

namespace BenchmarkClassic
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new BenchmarkJson();
            var t2 = b.Routine();
            var t1 = b.JsonNet();
            var t3 = b.ServiceStack1();
            BenchmarkRunner.Run<BenchmarkJson>();
        }
    }
}
