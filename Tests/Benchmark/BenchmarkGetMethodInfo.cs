using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using Vse.Routines.Json;

namespace Benchmark
{
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkGetMethodInfo
    {
        Func<StringBuilder, bool, bool> func1;
        Func<StringBuilder, bool, bool> func2;

        public BenchmarkGetMethodInfo()
        {
            var methodInfo2 = TrainJsonTools.GetMethodInfoExpr<bool>((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeBool(sb, t));
            var del2 = methodInfo2.CreateDelegate(typeof(Func<StringBuilder, bool, bool>));
            func2 = (Func<StringBuilder, bool, bool>)del2;


            //Func<StringBuilder, bool, bool> f = (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeBool(sb, t);
            //var methodInfo1 = f.GetMethodInfo();
            var methodInfo1 = TrainJsonTools.GetMethodInfo<bool>((sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeBool(sb, t));
            var del1 = methodInfo1.CreateDelegate(typeof(Func<StringBuilder, bool, bool>),null);
            func1 = (Func<StringBuilder, bool, bool>)del1;

        }
        [Benchmark]
        public bool DelegateBuilded()
        {
            var sb = new StringBuilder(200);
            return func1(sb, true);
        }

        [Benchmark]
        public bool Expression()
        {
            var sb = new StringBuilder(200);
            return func2(sb, true);
        }
    }
}
