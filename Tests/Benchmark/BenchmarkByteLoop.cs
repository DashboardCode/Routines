using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(MyManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkByteLoop
    {
        static byte[] bytes8000;
        static List<byte> bytelist8000;

        static string[] strings8000;
        static List<string> stringlist8000;

        static BenchmarkByteLoop()
        {
            var rnd = new Random();
            bytelist8000 = new List<byte>();
            stringlist8000 = new List<string>();
            for (int i=0;i<8002;i++)
            {
                bytelist8000.Add((byte)rnd.Next(0,255));
                stringlist8000.Add((Convert.ToBase64String(new byte[] { (byte)rnd.Next(0, 255) , (byte)rnd.Next(0, 255) , (byte)rnd.Next(0, 255) , (byte)rnd.Next(0, 255) , (byte)rnd.Next(0, 255) })));
            }
            bytes8000 = bytelist8000.ToArray();
            strings8000 = stringlist8000.ToArray();
        }

        #region bytes
        [Benchmark]
        public int B8000_Foreach()
        {
            var sum = 0;
            foreach(var b in bytes8000)
            {
                sum = sum + b;
            }
            return sum;
        }

        [Benchmark]
        public int B8000_For()
        {
            var sum = 0;
            var l = bytes8000.Length;
            for (var i = 0; i < l; i++)
            {
                sum = sum + bytes8000[i];
            }
            return sum;
        }

        [Benchmark]
        public int B8000_Enumerator()
        {
            var sum = 0;
            var e = bytes8000.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sum = sum + (byte)e.Current; 
                moveNext = e.MoveNext();
            }
            return sum;
        }

        [Benchmark]
        public int B8000_ListEnumerator()
        {
            var sum = 0;
            var e = bytelist8000.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sum = sum + e.Current;
                moveNext = e.MoveNext();
            }
            return sum;
        }
        #endregion

        #region strings
        [Benchmark]
        public int S8000_Foreach()
        {
            var sum = 0;
            foreach (var b in strings8000)
            {
                sum = sum + b.Length;
            }
            return sum;
        }

        [Benchmark]
        public int S8000_For()
        {
            var sum = 0;
            var l = strings8000.Length;
            for (var i = 0; i < l; i++)
            {
                sum = sum + strings8000[i].Length;
            }
            return sum;
        }

        [Benchmark]
        public int S8000_Enumerator()
        {
            var sum = 0;
            var e = strings8000.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sum = sum + ((string)e.Current).Length;
                moveNext = e.MoveNext();
            }
            return sum;
        }

        [Benchmark]
        public int S8000_ListEnumerator()
        {
            var sum = 0;
            var e = stringlist8000.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sum = sum + e.Current.Length;
                moveNext = e.MoveNext();
            }
            return sum;
        }
        #endregion
    }
}