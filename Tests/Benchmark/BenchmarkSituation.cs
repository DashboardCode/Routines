using System;
using System.Text;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using DashboardCode.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkSituation
    {
        static byte[] bytes8000;
        static byte[] bytes64;
        static BenchmarkSituation()
        {
            var rnd = new Random();
            var b1 = new List<byte>();
            for(int i=0;i<8002;i++)
            {
                b1.Add((byte)rnd.Next(0,255));
            }
            bytes8000 = b1.ToArray();

            var b2 = new List<byte>();
            for (int i = 0; i < 64; i++)
            {
                b2.Add((byte)rnd.Next(0, 255));
            }
            bytes64 = b2.ToArray();
        }

        [Benchmark]
        public string B8000SerializeBytesToJsonArray()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBytesToJsonArray(sb, bytes8000);
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string B0064SerializeBytesToJsonArray()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBytesToJsonArray(sb, bytes64);
            var text = sb.ToString();
            return text;
        }

       
        
    }
}
