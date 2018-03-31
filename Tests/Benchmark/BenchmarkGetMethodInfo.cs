using System;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

using DashboardCode.Routines.Json;

namespace Benchmark
{
    [Config(typeof(MyManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkGetMethodInfo
    {
        Func<StringBuilder, bool, bool> func1;
        Func<StringBuilder, bool, bool> func2;

        public BenchmarkGetMethodInfo()
        {
            var methodInfo2 = JsonChainTools.GetMethodInfoExpr<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t));
            var del2 = methodInfo2.CreateDelegate(typeof(Func<StringBuilder, bool, bool>));
            func2 = (Func<StringBuilder, bool, bool>)del2;


            //Func<StringBuilder, bool, bool> f = (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeBool(sb, t);
            //var methodInfo1 = f.GetMethodInfo();
            var methodInfo1 = GetMethodInfo<bool>((sb, t) => JsonValueStringBuilderExtensions.SerializeBool(sb, t));
            var del1 = methodInfo1.CreateDelegate(typeof(Func<StringBuilder, bool, bool>),null);
            func1 = (Func<StringBuilder, bool, bool>)del1;

        }

        public static MethodInfo GetMethodInfo<T>(Func<StringBuilder, T, bool> func)
        {
            var methodInfo = func.GetMethodInfo();
            return methodInfo;
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