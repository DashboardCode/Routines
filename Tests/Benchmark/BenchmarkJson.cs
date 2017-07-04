using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Vse.Routines;
using Vse.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkJson
    {
        static Box box;
        static List<Row> testData = new List<Row>();
        static Func<StringBuilder, Box, bool> serializer1;
        static Func<StringBuilder, Box, bool> serializer2;
        static Func<StringBuilder, Box, bool> serializer4;
        //static NExpJsonSerializer<Box> serializer3;
        static BenchmarkJson()
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

            Include<Box> include = (i) => i.IncludeAll(e => e.Rows);
            var include2 = IncludeExtensions.AppendLeafs(include);

            var process = new ChainVisitor<Box>();
            var chain = new Chain<Box>(process);
            include2.Invoke(chain);
            var serializerNode = process.Root;

            serializer1 = NavigationToJsonTools.BuildSerializer<Box>(serializerNode);

            Expression<Func<StringBuilder, Box, bool>> serializer2Exp = 
                    (sbP, tP) => NExpJsonSerializerStringBuilderExtensions.SerializeObject(sbP, tP,
                        (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Rows",  o => o.Rows,
                            (sb2, t2) => NExpJsonSerializerStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    NExpJsonSerializerStringBuilderExtensions.SerializeObject(sb3, t3,
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "At", o => o.At, NExpJsonSerializerFormatters.SerializeToIso8601WithMs),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "I1", o => o.I1, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "I2", o => o.I2, NExpJsonSerializerStringBuilderExtensions.SerializeStruct, NExpJsonSerializerStringBuilderExtensions.SerializeNull),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", o => o.B1, NExpJsonSerializerStringBuilderExtensions.SerializeBool),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "B2", o => o.B2, NExpJsonSerializerStringBuilderExtensions.SerializeBool, NExpJsonSerializerStringBuilderExtensions.SerializeNull),

                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "D1", o => o.D1, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "D2", o => o.D2, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "D3", o => o.D3, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "D4", o => o.D4, NExpJsonSerializerStringBuilderExtensions.SerializeStruct, NExpJsonSerializerStringBuilderExtensions.SerializeNull),

                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "F1", o => o.F1, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "F2", o => o.F2, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "F3", o => o.F3, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "F4", o => o.F4, NExpJsonSerializerStringBuilderExtensions.SerializeStruct, NExpJsonSerializerStringBuilderExtensions.SerializeNull)
                                     ),
                                NExpJsonSerializerStringBuilderExtensions.SerializeNull
                              ),
                            NExpJsonSerializerStringBuilderExtensions.SerializeNull
                        )
                    );
            serializer2 = serializer2Exp.Compile();

            serializer4 = (sbP, tP) => NExpJsonSerializerStringBuilderExtensions.SerializeObject(sbP, tP,
                        (sb, t) => NExpJsonSerializerStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Rows", o => o.Rows,
                            (sb2, t2) => NExpJsonSerializerStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    NExpJsonSerializerStringBuilderExtensions.SerializeObject(sb3, t3,
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "At", o => o.At, NExpJsonSerializerFormatters.SerializeToIso8601WithMs),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "I1", o => o.I1, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "I2", o => o.I2, NExpJsonSerializerStringBuilderExtensions.SerializeStruct, NExpJsonSerializerStringBuilderExtensions.SerializeNull),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", o => o.B1, NExpJsonSerializerStringBuilderExtensions.SerializeBool),
                                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "B2", o => o.B2, NExpJsonSerializerStringBuilderExtensions.SerializeBool, NExpJsonSerializerStringBuilderExtensions.SerializeNull),

                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "D1", o => o.D1, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "D2", o => o.D2, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "D3", o => o.D3, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "D4", o => o.D4, NExpJsonSerializerStringBuilderExtensions.SerializeStruct, NExpJsonSerializerStringBuilderExtensions.SerializeNull),

                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "F1", o => o.F1, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "F2", o => o.F2, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "F3", o => o.F3, NExpJsonSerializerStringBuilderExtensions.SerializeStruct),
                                     (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeNStructPropertyHandleNull(sb4, t4, "F4", o => o.F4, NExpJsonSerializerStringBuilderExtensions.SerializeStruct, NExpJsonSerializerStringBuilderExtensions.SerializeNull)
                                     ),
                                NExpJsonSerializerStringBuilderExtensions.SerializeNull
                              ),
                            NExpJsonSerializerStringBuilderExtensions.SerializeNull
                        ));

            Include < Box> includeAlt = (i) => i.IncludeAll(e => e.Rows);
            //serializer3 = includeAlt.BuildNExpJsonSerializer();
        }

        //[Benchmark]
        public string RoutineExpression()
        {
            var sb = new StringBuilder(4000);
            serializer2(sb, box);
            var json = sb.ToString();
            return json;
        }


        //[Benchmark]
        //public string RoutineInterpretated()
        //{
        //    var text = serializer3.Serialize(box);
        //    return text;
        //}

        [Benchmark]
        public string RoutineExpressionCompiled()
        {
            var sb = new StringBuilder(4000);
            serializer1(sb, box);
            var json = sb.ToString();
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

        [Benchmark]
        public string JsonNet()
        {
            string text = JsonConvert.SerializeObject(
                box,
                new JsonSerializerSettings
                {
                     //DateFormatString= "yyyy-MM-ddTHH:mm:ss.fffK" //"yyyy-MM-ddTHH:mm:ssK", 
                     //NullValueHandling= NullValueHandling.Ignore
                });
            return text;
        }
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
