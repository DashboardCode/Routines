using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using DashboardCode.Routines;
using DashboardCode.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkCharBuffer
    {
        static byte[] bytes8000;
        static byte[] bytes64;
        static BenchmarkCharBuffer()
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
        public string StringBuilder8000()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBytesToJsonArray(sb, bytes8000);
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string StringBuilder64()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBytesToJsonArray(sb, bytes64);
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string CharBuffer8000()
        {
            var sb = new StringBuilder();
            SerializeBytesToJsonArray(sb, bytes8000);
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string CharBuffer64()
        {
            var sb = new StringBuilder();
            SerializeBytesToJsonArray(sb, bytes64);
            var text = sb.ToString();
            return text;
        }

        private static bool SerializeBytesToJsonArray(StringBuilder sb, byte[] bytes)
        {
            var length = bytes.Length;
            var buffer = new char[length * 3 + 2 + (length == 0 ? 0 : length - 1)];
            int bufferPosition = 0;
            buffer[bufferPosition++] = '[';
            for (var i = 0; i < length; i++)
            {
                var chars = bytes[i].ToString();
                var cl = chars.Length;
                chars.CopyTo(0, buffer, bufferPosition, cl);
                bufferPosition += cl;
                buffer[bufferPosition++] = ',';
            }
            if (bufferPosition == 1)
                buffer[bufferPosition++] = ']';
            else
                buffer[bufferPosition - 1] = ']'; ;
            sb.Append(buffer, 0, bufferPosition);
            return true;
        }
        
    }
}
