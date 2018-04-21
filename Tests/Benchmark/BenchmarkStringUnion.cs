﻿using System;
using System.Text;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkStringUnion
    {
        List<string> testData = new List<string>();
        public BenchmarkStringUnion()
        {
            for(int i=0;i<1000;i++)
            {
                testData.Add(i.ToString());
            }
        }
        [Benchmark]
        public string StringJoin()
        {
            var text = string.Join<string>(",", testData);
            return text;
        }
        [Benchmark]
        public string SeparatorSubstitution()
        {
            var sb = new StringBuilder();
            string separator = String.Empty;
            foreach (var value in testData)
            {
                sb.Append(separator).Append(value);
                separator = ",";
            }
            return sb.ToString();
        }
        [Benchmark]
        public string SeparatorStepBack()
        {
            var sb = new StringBuilder();
            foreach (var item in testData)
            {
                sb.Append(item).Append(',');
            }
            if (sb.Length >= 1)
                    sb.Length--;
            return sb.ToString();
        }
        [Benchmark]
        public string SeparatorStepBack2()
        {
            var sb = new StringBuilder();
            foreach (var item in testData)
            {
                sb.Append(item).Append(',');
            }
            if (sb[sb.Length-1] == ',')
                sb.Length--;
            return sb.ToString();
        }
        [Benchmark]
        public string Enumerable()
        {
            var sb = new StringBuilder();
            var e = testData.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sb.Append(e.Current);
                moveNext = e.MoveNext();
                if (moveNext)
                        sb.Append(",");
            }
            return sb.ToString();
        }
    }
}
