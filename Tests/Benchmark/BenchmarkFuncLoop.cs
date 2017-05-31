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
    public class BenchmarkFuncLoop
    {
        static Func<StringBuilder, Box ,bool>[] func16;
        static List<Func<StringBuilder, Box, bool>> funclist16;

        static BenchmarkFuncLoop()
        {
            var rnd = new Random();
            funclist16 = new List<Func<StringBuilder, Box, bool>>();
            for (int i=0; i<8002; i++)
            {
                Func<StringBuilder, Box, bool> f = (sb,b)=>true;
                funclist16.Add(f);
            }
            func16 = funclist16.ToArray();
        }

        #region bytes
        [Benchmark]
        public bool F16_Foreach()
        {
            var sum = false;
            foreach(var f in func16)
            {
                var notEmpty = f(null,null);
                if (notEmpty)
                    if (!sum)
                        sum = true;
            }
            return sum;
        }

        [Benchmark]
        public bool F16_For()
        {
            var sum = false;
            var l = func16.Length;
            for (var i = 0; i < l; i++)
            {
                var notEmpty = func16[i](null, null);
                if (notEmpty)
                    if (!sum)
                        sum = true;
            }
            return sum;
        }

        [Benchmark]
        public bool F16_Enumerator()
        {
            var sum = false;
            var e = func16.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                var notEmpty = ((Func<StringBuilder, Box, bool>)e.Current)(null, null);
                if (notEmpty)
                    if (!sum)
                        sum = true;
                moveNext = e.MoveNext();
            }
            return sum;
        }

        [Benchmark]
        public bool F16_ListEnumerator()
        {
            var sum = false;
            var e = funclist16.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                var notEmpty = e.Current(null, null);
                if (notEmpty)
                    if (!sum)
                        sum = true;
                moveNext = e.MoveNext();
            }
            return sum;
        }
        #endregion

    }
}
