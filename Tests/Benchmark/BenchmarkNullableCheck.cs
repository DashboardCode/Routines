using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using System;

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

        #region implementation
        public static bool CheckObjectImpl(object o)
        {
            return o != null;
        }

        public static bool CheckGenericImpl<T> (T t)
        {
            return t != null;
        }

        public static bool CheckNullableImpl<T>(T? o) where T: struct
        {
            return o.HasValue;
        }

        public static bool CheckNullableEqImpl<T>(T? o) where T : struct
        {
            return o != null;
        }
        #endregion



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
        public bool CheckGeneric()
        {
            return CheckGenericImpl(x);
        }

        [Benchmark]
        public bool CheckNullableEq()
        {
            return CheckNullableEqImpl(x);
        }
    }
}
