using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using Vse.Routines;
using Vse.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkNullableCheck
    {
        static int? x = (new Random()).Next();

        public static bool CheckObjectImpl(object o)
        {
            return o != null;
        }

        public static bool CheckNullableImpl<T>(T? o) where T: struct
        {
            return o.HasValue;
        }


        public static bool CheckNullableEqImpl<T>(T? o) where T : struct
        {
            return o!=null;
        }

        [Benchmark]
        public bool CheckObject()
        {
            return CheckObjectImpl(x);
        }

        [Benchmark]
        public bool CheckNullable()
        {
            return CheckNullableImpl(x);
        }

        [Benchmark]
        public bool CheckNullableEq()
        {
            return CheckNullableEqImpl(x);
        }
    }
}
