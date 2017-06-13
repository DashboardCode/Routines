using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using System;
using System.Globalization;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkStringConvert
    {
        decimal d = ((decimal)1)/3;
        float f = ((float)1) / 3;

        [Benchmark]
        public string StringDecimal()
        {
            var text = d.ToString();
            return text;
        }

        [Benchmark]
        public string StringFloat()
        {
            var text = f.ToString();
            return text;
        }

        [Benchmark]
        public string StringDecimalConvert()
        {
            var text = Convert.ToString(d);
            return text;
        }

        [Benchmark]
        public string StringFloatConvert()
        {
            var text = Convert.ToString(f);
            return text;
        }

        [Benchmark]
        public string StringDecimalConvertInvariant()
        {
            var text = Convert.ToString(d, CultureInfo.InvariantCulture);
            return text;
        }

        [Benchmark]
        public string StringFloatConvertInvariant()
        {
            var text = Convert.ToString(f, CultureInfo.InvariantCulture);
            return text;
        }
    }
}
