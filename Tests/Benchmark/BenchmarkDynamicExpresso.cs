using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using DynamicExpresso;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkDynamicExpresso
    { 

        [Benchmark]
        public object TestAdminkaStyle()
        {
            var input = new { f1 = "", f2 = "" };
            object oInput = input;
            var output = new List<int> { 1,2,3,4,5};
            object oOutput = output;
            var timeSpan = TimeSpan.FromSeconds(2);
            var parameters = new[] {
                new Parameter("input", oInput),
                new Parameter("output", oOutput),
                new Parameter("timeSpan", timeSpan)
            };
            var interpreter = new Interpreter();
            var result = interpreter.Eval("output.Count()==5", parameters);
            return result;
        }

        //[Benchmark]
        //public object TestEvalArithmetics()
        //{
        //    var interpreter = new Interpreter();
        //    var result = interpreter.Eval("8 / 2 + 2");
        //    return result;
        //}

        //[Benchmark]
        //public int TestEvalCompile()
        //{
        //    var prices = new[] { 5, 8, 6, 2 };
        //    var whereFunction = new Interpreter().ParseAsDelegate<Func<int, bool>>("arg > 5");
        //    var count = prices.Where(whereFunction).Count();
        //    return count;
        //}
    }
}