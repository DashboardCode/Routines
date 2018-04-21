using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkDateTimeFormat
    {
        public static DateTime dateTime = DateTime.Now;

        [Benchmark]
        public string CustomDev1()
        {
            var d = dateTime.ToUniversalTime();
            var sb = new StringBuilder(20);

            sb.Append(d.Year).Append("-");
            if (d.Month < 10)
                sb.Append("0");
            sb.Append(d.Month).Append("-");
            if (d.Day < 10)
                sb.Append("0");
            sb.Append(d.Day).Append("T");
            if (d.Hour < 10)
                sb.Append("0");
            sb.Append(d.Hour).Append(":");
            if (d.Minute < 10)
                sb.Append("0");
            sb.Append(d.Minute).Append(":");
            if (d.Second < 10)
                sb.Append("0");
            sb.Append(d.Second).Append("Z");
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string CustomDev2()
        {
            var u = dateTime.ToUniversalTime();
            var sb = new StringBuilder(20);
            var y = u.Year;
            var d = u.Day;
            var M = u.Month;
            var h = u.Hour;
            var m = u.Minute;
            var s = u.Second;
            sb.Append(y).Append("-");
            if (M <= 9)
                sb.Append("0");
            sb.Append(M).Append("-");
            if (d <= 9)
                sb.Append("0");
            sb.Append(d).Append("T");
            if (h <= 9)
                sb.Append("0");
            sb.Append(h).Append(":");
            if (m <= 9)
                sb.Append("0");
            sb.Append(m).Append(":");
            if (s <= 9)
                sb.Append("0");
            sb.Append(s).Append("Z");
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string CustomDev2b()
        {
            var u = dateTime.ToUniversalTime();
            var sb = new StringBuilder(21);
            var y = u.Year;
            var d = u.Day;
            var M = u.Month;
            var h = u.Hour;
            var m = u.Minute;
            var s = u.Second;
            sb.Append(y).Append("-");
            if (M <= 9)
                sb.Append("0");
            sb.Append(M).Append("-");
            if (d <= 9)
                sb.Append("0");
            sb.Append(d).Append("T");
            if (h <= 9)
                sb.Append("0");
            sb.Append(h).Append(":");
            if (m <= 9)
                sb.Append("0");
            sb.Append(m).Append(":");
            if (s <= 9)
                sb.Append("0");
            sb.Append(s).Append("Z");
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string CustomDev2WithMS()
        {
            var u  = dateTime.ToUniversalTime();
            var sb = new StringBuilder(23);
            var y  = u.Year;
            var d  = u.Day;
            var M  = u.Month;
            var h  = u.Hour;
            var m  = u.Minute;
            var s  = u.Second;
            var ms = u.Millisecond;
            sb.Append(y).Append("-");
            if (M <= 9)
                sb.Append("0");
            sb.Append(M).Append("-");
            if (d <= 9)
                sb.Append("0");
            sb.Append(d).Append("T");
            if (h <= 9)
                sb.Append("0");
            sb.Append(h).Append(":");
            if (m <= 9)
                sb.Append("0");
            sb.Append(m).Append(":");
            if (s <= 9)
                sb.Append("0");
            sb.Append(s).Append(".");
            sb.Append(ms).Append("Z");
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string CustomDev2WithMS2()
        {
            var u = dateTime.ToUniversalTime();
            var sb = new StringBuilder(25);
            var y = u.Year;
            var d = u.Day;
            var M = u.Month;
            var h = u.Hour;
            var m = u.Minute;
            var s = u.Second;
            var ms = u.Millisecond;
            sb.Append(y).Append("-");
            if (M <= 9)
                sb.Append("0");
            sb.Append(M).Append("-");
            if (d <= 9)
                sb.Append("0");
            sb.Append(d).Append("T");
            if (h <= 9)
                sb.Append("0");
            sb.Append(h).Append(":");
            if (m <= 9)
                sb.Append("0");
            sb.Append(m).Append(":");
            if (s <= 9)
                sb.Append("0");
            sb.Append(s).Append(".");
            sb.Append(ms).Append("Z");
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string FormatO()
        {
            var text = dateTime.ToUniversalTime().ToString("o");
            return text;
        }
        [Benchmark]
        public string FormatS()
        {
            var text = string.Concat(dateTime.ToUniversalTime().ToString("s"),"Z");
            return text;
        }

        [Benchmark]
        public string Verify_FormatS()
        {
            var text = string.Concat(dateTime.ToUniversalTime().ToString("s"), "Z");
            return text;
        }

        [Benchmark]
        public string CustomFormatK()
        {
            var text = dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            return text;
        }

        [Benchmark]
        public string CustomFormatKfff()
        {
            var text = dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffK");
            return text;
        }

        [Benchmark]
        public string Verify_CustomFormatK()
        {
            var text = dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            return text;
        }
    }
}
