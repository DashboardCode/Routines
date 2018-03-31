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
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkListAccess
    {
        public class TestData
        {
            public int Key { get; set; }
            public string Value { get; set; }

            
        }
        static Dictionary<int, TestData> testData10 = new Dictionary<int, TestData>();
        static Dictionary<int, TestData> testData100 = new Dictionary<int, TestData>();
        static Dictionary<int, TestData> testData1000 = new Dictionary<int, TestData>();
        static Dictionary<int, TestData> testData10000 = new Dictionary<int, TestData>();
        static List<TestData> testList10 = new List<TestData>();
        static List<TestData> testList100 = new List<TestData>();
        static List<TestData> testList1000 = new List<TestData>();
        static List<TestData> testList10000 = new List<TestData>();
        static Random rnd = new Random();

        static BenchmarkListAccess()
        {

            for (int i = 0; i < 10000; i++)
            {
                var t = new TestData() { Key = i, Value = "test" };
                if (i < 10)
                {
                    testData10.Add(t.Key, t);
                    testData100.Add(t.Key, t);
                    testData1000.Add(t.Key, t);
                    testData10000.Add(t.Key, t);

                    testList10.Add(t);
                    testList100.Add(t);
                    testList1000.Add(t);
                    testList10000.Add(t);
                    continue;
                }
                if (i < 100)
                {
                    testData100.Add(t.Key, t);
                    testData1000.Add(t.Key, t);
                    testData10000.Add(t.Key, t);

                    testList100.Add(t);
                    testList1000.Add(t);
                    testList10000.Add(t);
                    continue;
                }
                if (i < 1000)
                {
                    testData1000.Add(t.Key, t);
                    testData10000.Add(t.Key, t);

                    testList1000.Add(t);
                    testList10000.Add(t);
                    continue;
                }
                if (i < 10000)
                {
                    testData10000.Add(t.Key, t);
                    testList10000.Add(t);
                    continue;
                }
            }
        }

        [Benchmark]
        public TestData Dictionary10()
        {
            int key = rnd.Next(0, 9);
            testData10.TryGetValue(key, out TestData t);
            return t;
        }

        [Benchmark]
        public TestData Dictionary100()
        {
            int key = rnd.Next(0, 99);
            testData100.TryGetValue(key, out TestData t);
            return t;
        }

        [Benchmark]
        public TestData Dictionary1000()
        {
            int key = rnd.Next(0, 999);
            testData1000.TryGetValue(key, out TestData t);
            return t;
        }

        [Benchmark]
        public TestData Dictionary10000()
        {
            int key = rnd.Next(0, 9999);
            testData10000.TryGetValue(key, out TestData t);
            return t;
        }

        [Benchmark]
        public TestData List10()
        {
            int key = rnd.Next(0, 9);
            var t = testList10.Find(e => e.Key == key);
            return t;
        }

        [Benchmark]
        public TestData List100()
        {
            int key = rnd.Next(0, 99);
            var t = testList100.Find(e => e.Key == key);
            return t;
        }

        [Benchmark]
        public TestData List1000()
        {
            int key = rnd.Next(0, 999);
            var t = testList1000.Find(e => e.Key == key);
            return t;
        }

        [Benchmark]
        public TestData List10000()
        {
            int key = rnd.Next(0, 9999);
            var t = testList10000.Find(e => e.Key == key);
            return t;
        }
    }
}