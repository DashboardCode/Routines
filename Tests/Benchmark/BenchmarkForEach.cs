﻿using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(MyManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkForEach
    {
        List<string> testData = new List<string>();
        string[] testArray;
        public BenchmarkForEach()
        {
            for(int i=0;i<1000;i++)
            {
                testData.Add(i.ToString());
            }
            testArray = testData.ToArray();
        }

        [Benchmark]
        public int TestList()
        {
            var x = 0;
            foreach(var i in testData)
            {
                x += i.Length;
            }
            return x;
        }

        [Benchmark]
        public int TestArray()
        {
            var x = 0;
            foreach (var i in testArray)
            {
                x += i.Length;
            }
            return x;
        }
    }
}