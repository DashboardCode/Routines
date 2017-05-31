//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Attributes.Columns;
//using BenchmarkDotNet.Attributes.Exporters;
//using BenchmarkDotNet.Attributes.Jobs;
//using BenchmarkDotNet.Diagnostics.Windows.Configs;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using Vse.Routines;
//using Vse.Routines.Json;

//namespace BenchmarkClassic
//{
//    //[Config(typeof(Config))]
//    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
//    [ClrJob/*, CoreJob*/]
//    [HtmlExporter, MarkdownExporter]
//    [MemoryDiagnoser, InliningDiagnoser]
//    public class BenchmarkJson2
//    {
//        public class Row
//        {
//            public DateTime At { get; set; }
//            public int I1 { get; set; }
//            public int? I2 { get; set; }
//            public bool B1 { get; set; }
//            public bool? B2 { get; set; }
//            public decimal D1 {get;set;}
//            public decimal D2 { get; set; }
//            public decimal D3 { get; set; }
//            public decimal? D4 { get; set; }
//            public double F1 { get; set; }
//            public double F2 { get; set; }
//            public double F3 { get; set; }
//            public double? F4 { get; set; }
//        }

//        static List<Row> testData = new List<Row>();
//        static NExpJsonSerializer<Row> serializer;
//        static BenchmarkJson2()
//        {
//            for(int i=0;i<6;i++)
//            {
//                testData.Add(new Row {
//                    At=DateTime.Now.ToUniversalTime(),
//                    I1 = 5,
//                    I2 = null,
//                    B1 = true,
//                    B2 = null,
//                    D1 = (decimal)0.21,
//                    D2 = (decimal)0.22,
//                    D3 = (decimal)0.23,
//                    D4 = null,
//                    F1 = 0.31,
//                    F2 = 0.32,
//                    F3 = 0.33,
//                    F4 = null
//                } );
//            }

//            Include<Row> includes = null;
//            var settings = new NExpJsonSerializerSettings
//            {
//                DateTimeFormatter = NExpJsonSerializerFormatters.SerializeToUnixTimeMs
//            };
//            serializer = includes.BuildNExpJsonSerializer(settings);
//        }

//        [Benchmark]
//        public string Routine()
//        {
//            var text = serializer.Serialize(testData);
//            return text;
//        }

//        [Benchmark]
//        public string ServiceStack1()
//        {
//            //var t = Test;
//            var json = ServiceStack.Text.JsonSerializer.SerializeToString(testData);
//            return json;
//        }

//        [Benchmark]
//        public string JsonNet()
//        {
//            string text = JsonConvert.SerializeObject(testData,/* Formatting.Indented,*/ 
//                new JsonSerializerSettings
//            {
//                 //DateFormatString= "yyyy-MM-ddTHH:mm:ssK",
//                 NullValueHandling= NullValueHandling.Ignore
//                //TypeNameHandling = TypeNameHandling.All
//            });
//            return text;
//        }
//    }
//}
