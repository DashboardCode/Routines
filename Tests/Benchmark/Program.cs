using BenchmarkDotNet.Running;
using System;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //var b = new BenchmarkJson();
            //var t2 = b.Routine();
            //var t1 = b.JsonNet();

            //var t4 = b.CustomDev2WithMS();
            //BenchmarkRunner.Run<BenchmarkRoutines>();
            //BenchmarkRunner.Run<BenchmarkStringUnion>();
            BenchmarkRunner.Run<BenchmarkJson>();
        }
    }
}