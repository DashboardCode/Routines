using BenchmarkDotNet.Running;
using System;
using Vse.Routines.Json;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //var b = new BenchmarkCharBuffer();
            //var s1 = b.CharBuffer64();
            //var s2 = b.StringBuilder64();
            //if (s1 != s2)
            //    throw new Exception("asdasd");
            BenchmarkRunner.Run<BenchmarkNullableCheck>();
        }
    }
}