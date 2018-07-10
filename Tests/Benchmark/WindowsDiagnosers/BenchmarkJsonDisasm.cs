﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

using DashboardCode.Routines;
using DashboardCode.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(ManualWindowsDiagnosersConfig))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
#if !(NETCOREAPP1_1  || NETCOREAPP2_0 || NETCOREAPP2_1)
    //[HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.BranchMispredictions, BenchmarkDotNet.Diagnosers.HardwareCounter.BranchInstructions)]
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    [BenchmarkDotNet.Attributes.Jobs.RyuJitX64Job]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.InliningDiagnoser]
#endif
    public class BenchmarkJsonDisasm
    {
        static Box box;
        static List<Row> testData = new List<Row>();
        static Func<Box, string> formatter1;
        static Func<StringBuilder, Box, bool> serializer2;
        static Func<StringBuilder, Box, bool> serializer4;

        static BenchmarkJsonDisasm()
        {
            for(int i=0;i<600;i++)
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
            box = new Box { Rows = testData };

            Include<Box> include = t => t.IncludeAll(e => e.Rows);
            var include2 = include.AppendLeafs(); 
            var parser = new ChainVisitor<Box>();
            var train = new Chain<Box>(parser);
            include2.Invoke(train);
            var serializerNode = parser.Root;

            formatter1 = JsonManager.ComposeFormatter<Box>(serializerNode.ComposeInclude<Box>());

            serializer4 = (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sbP, tP,
                        (sb, t) => JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Rows", o => o.Rows,
                            (sb2, t2) => JsonComplexStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sb3, t3,
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "At", o => o.At, JsonValueStringBuilderExtensions.SerializeToIso8601WithMs),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "I1", o => o.I1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "I2", o => o.I2, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "B1", o => o.B1, JsonValueStringBuilderExtensions.SerializeBool),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "B2", o => o.B2, JsonValueStringBuilderExtensions.SerializeBool, JsonValueStringBuilderExtensions.NullSerializer),

                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D1", o => o.D1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D2", o => o.D2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D3", o => o.D3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "D4", o => o.D4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),

                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F1", o => o.F1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F2", o => o.F2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F3", o => o.F3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "F4", o => o.F4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer)
                                     ),
                                JsonValueStringBuilderExtensions.NullSerializer
                              ),
                            JsonValueStringBuilderExtensions.NullSerializer
                        ));

            Expression<Func<StringBuilder, Box, bool>> serializer2Exp = 
                    (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sbP, tP,
                        (sb, t) => JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Rows",  o => o.Rows,
                            (sb2, t2) => JsonComplexStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sb3, t3,
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "At", o => o.At, JsonValueStringBuilderExtensions.SerializeToIso8601WithMs),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "I1", o => o.I1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "I2", o => o.I2, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "B1", o => o.B1, JsonValueStringBuilderExtensions.SerializeBool),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "B2", o => o.B2, JsonValueStringBuilderExtensions.SerializeBool, JsonValueStringBuilderExtensions.NullSerializer),

                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D1", o => o.D1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D2", o => o.D2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D3", o => o.D3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "D4", o => o.D4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),

                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F1", o => o.F1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F2", o => o.F2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F3", o => o.F3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "F4", o => o.F4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer)
                                     ),
                                JsonValueStringBuilderExtensions.NullSerializer
                              ),
                            JsonValueStringBuilderExtensions.NullSerializer
                        )
                    );
            serializer2 = serializer2Exp.Compile();

            
        }

        [Benchmark]
        public string RoutineExpression()
        {
            var sb = new StringBuilder(4000);
            serializer2(sb, box);
            var json = sb.ToString();
            return json;
        }

        [Benchmark]
        public string SerializeToString()
        {
            var json = ServiceStack.Text.JsonSerializer.SerializeToString(testData);
            return json;
        }

        [Benchmark]
        public string RoutineExpressionCompiled()
        {
            var json = formatter1(box);
            return json;
        }

        [Benchmark]
        public string RoutineFunc()
        {
            var sb = new StringBuilder(4000);
            serializer4(sb, box);
            var json = sb.ToString();
            return json;
        }

        #region JsonNet
        [Benchmark]
        public string JsonNet()
        {
            string text = JsonConvert.SerializeObject(
                box,
                new Newtonsoft.Json.JsonSerializerSettings { });
            return text;
        }

        [Benchmark]
        public string JsonNet_Indented()
        {
            string text = JsonConvert.SerializeObject(
                box, Formatting.Indented,
                new Newtonsoft.Json.JsonSerializerSettings { });
            return text;
        }

        [Benchmark]
        public string JsonNet_NullIgnore()
        {
            string text = JsonConvert.SerializeObject(
                box,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            return text;
        }

        [Benchmark]
        public string JsonNet_DateFormatFF()
        {
            string text = JsonConvert.SerializeObject(
                box,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    DateFormatString= "yyyy-MM-ddTHH:mm:ssK"
                });
            return text;
        }

        [Benchmark]
        public string JsonNet_DateFormatSS()
        {
            string text = JsonConvert.SerializeObject(
                box,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK"
                });
            return text;
        }
        #endregion

        [Benchmark]
        public string ServiceStack1()
        {
            var json = ServiceStack.Text.JsonSerializer.SerializeToString(box);
            return json;
        }

        public class Box
        {
            public List<Row> Rows { get; set; }
        }

        public class Row
        {
            public DateTime At { get; set; }
            public int I1 { get; set; }
            public int? I2 { get; set; }
            public bool B1 { get; set; }
            public bool? B2 { get; set; }
            public decimal D1 { get; set; }
            public decimal D2 { get; set; }
            public decimal D3 { get; set; }
            public decimal? D4 { get; set; }
            public double F1 { get; set; }
            public double F2 { get; set; }
            public double F3 { get; set; }
            public double? F4 { get; set; }
        }
    }
}