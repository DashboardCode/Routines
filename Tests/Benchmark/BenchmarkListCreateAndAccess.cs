using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Vse.Routines;
using Vse.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkListCreateAndAccess
    {
        public class TestData
        {
            public int Key { get; set; }
            public string Value { get; set; }
        }
        static List<TestData> testList20 = new List<TestData>();
        static Random rnd = new Random();

        static BenchmarkListCreateAndAccess()
        {
            for (int i = 0; i < 10; i++)
            {
                var t = new TestData() { Key = i, Value = "test" };
                testList20.Add(t);
                testList20.Add(t);

            }

            for (var i = 0; i < testList20.Count; i++)
            {
                var x = testList20[rnd.Next(0, 19)];
                if (x != null)
                {
                    testList20.Remove(x);
                    testList20.Add(x);
                }
            }
        }
        #region Dictionary
        [Benchmark]
        public Dictionary<int, TestData> Dictionary_10from20()
        {
            var d = new Dictionary<int, TestData>();
            foreach (var i in testList20 )
            {
                if (!d.TryGetValue(i.Key, out TestData t))
                {
                    d.Add(i.Key, t);
                }
            }
            return d;
        }
        #endregion 

        #region list
        [Benchmark]
        public List<TestData> List_10from20()
        {
            var d = new List<TestData>();
            foreach (var i in testList20)
            {
                var x = d.Find(e => e.Key == i.Key);
                if (x==null)
                {
                    d.Add(i);
                }
            }
            return d;
        }
        #endregion

    }
}
