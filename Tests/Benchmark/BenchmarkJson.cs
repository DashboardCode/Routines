using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Vse.Routines;
using Vse.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn/*, RankColumn*/]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkJson
    {
        public class Row
        {
            public DateTime At { get; set; }
            public int I1 { get; set; }
            public int? I2 { get; set; }
            public bool B1 { get; set; }
            public bool? B2 { get; set; }
            public decimal D1 {get;set;}
            public decimal D2 { get; set; }
            public decimal D3 { get; set; }
            public decimal? D4 { get; set; }
            public double F1 { get; set; }
            public double F2 { get; set; }
            public double F3 { get; set; }
            public double? F4 { get; set; }
        }

        static List<Row> testData = new List<Row>();
        static NavigationExpressionJsonSerializer<Row> serializer;
        static BenchmarkJson()
        {
            for(int i=0;i<6000;i++)
            {
                testData.Add(new Row {
                    At=DateTime.Now,
                    I1 = 5,
                    I2 = null,
                    B1 = true,
                    B2 = null,
                    D1 = (decimal)0.21,
                    D2 = (decimal)0.22,
                    D3 = (decimal)0.23,
                    D4 = null,
                    F1 = 0.31,
                    F2 = 0.32,
                    F3 = 0.33,
                    F4 = null
                } );
            }

            Include<Row> includes = null;
            serializer = includes.BuildNavigationExpressionJsonSerializer();
        }

        [Benchmark]
        public string Routine()
        {
            var text = serializer.Serialize(testData);
            return text;
        }

        [Benchmark]
        public string JsonNet()
        {
            string text = JsonConvert.SerializeObject(testData,/* Formatting.Indented,*/ new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.All
            });
            return text;
        }
    }
}
