using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using BenchmarkDotNet.Attributes;

using DashboardCode.Routines;
using DashboardCode.Routines.Json;
using Jil;
using FastExpressionCompiler;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkComposeFormatter
    {
        static Box box;
        static List<Row> testData = new List<Row>();
        static Func<Box, string> composeFormatterDelegate;
        static Func<Box, string> composeFormatterFastCompileDelegate;
        static Func<StringBuilder, Box, bool> fastExpressionCompilerDelegate;
        static Func<StringBuilder, Box, bool> dslRoutineExpressionManuallyConstruted;
        static Func<StringBuilder, Box, bool> dslRoutineDelegateManuallyConstrutedFormatter;

        public static string ImperativeIdeal(Box box)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"Rows\"}:");
            sb.Append("[");
            var added = false;
            foreach(var o in box.Rows)
            {
                sb.Append("{");
                sb.Append("\"At\":").Append('"').Append(o.At.ToString("yyyy-MM-ddTHH:mm:ss.fffK")).Append('"').Append(",");
                sb.Append("\"I1\":").Append(o.I1).Append(",");
                sb.Append("\"I2\":");
                if (o.I2.HasValue)
                    sb.Append(o.I2.Value); 
                else
                    sb.Append("null");
                sb.Append(",");
                sb.Append("\"B1\":").Append(o.B1).Append(",");
                sb.Append("\"B2\":");
                if (o.B2.HasValue)
                    sb.Append(o.B2.Value);
                else
                    sb.Append("null");
                sb.Append(",");

                sb.Append("\"D1\":").Append(o.D1).Append(",");
                sb.Append("\"D2\":").Append(o.D2).Append(",");
                sb.Append("\"D3\":").Append(o.D3).Append(",");
                sb.Append("\"D4\":");
                if (o.D4.HasValue)
                    sb.Append(o.D4.Value);
                else
                    sb.Append("null");
                sb.Append(",");

                sb.Append("\"F1\":").Append(o.F1); sb.Append(",");
                sb.Append("\"F2\":").Append(o.F2); sb.Append(",");
                sb.Append("\"F3\":").Append(o.F3); sb.Append(",");
                sb.Append("\"F4\":").Append(",");
                if (o.F4.HasValue)
                    sb.Append(o.F4.Value);
                else
                    sb.Append("null");

                sb.Append("},");
                added = true;
            }
            if (added)
                sb.Length = sb.Length - 1;
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        static BenchmarkComposeFormatter()
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


            Include<Box> include = (chain => chain.IncludeAll(e => e.Rows));
            var includeWithLeafs = include.AppendLeafs();

            composeFormatterDelegate = JsonManager.ComposeFormatter(includeWithLeafs);
            composeFormatterFastCompileDelegate = JsonManager.ComposeFormatter(includeWithLeafs, compile: (ex)=>ex.CompileFast());

            Expression<Func<StringBuilder, Box, bool>> dslRoutineExpressionManuallyConstrutedExpression = 
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

            dslRoutineExpressionManuallyConstruted = dslRoutineExpressionManuallyConstrutedExpression.Compile();
            fastExpressionCompilerDelegate = dslRoutineExpressionManuallyConstrutedExpression.CompileFast();

            dslRoutineDelegateManuallyConstrutedFormatter = (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sbP, tP,
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
        }

        [Benchmark]
        public string fastExpressionCompiler()
        {
            var sb = new StringBuilder();
            fastExpressionCompilerDelegate(sb, box);
            return sb.ToString();
        }


        [Benchmark]
        public string dslComposeFormatter()
        {
            var json = composeFormatterDelegate(box);
            return json;
        }

        [Benchmark]
        public string dslComposeFormatter_FastCompile()
        {
            var json = composeFormatterFastCompileDelegate(box);
            return json;
        }

        [Benchmark]
        public string ImperativeIdeal()
        {
            return ImperativeIdeal(box);
        }
        
        [Benchmark]
        public string jil()
        {
            using (var output = new System.IO.StringWriter())
            {
                JSON.Serialize(box, output, new Options(excludeNulls:false) );
                return output.ToString();
            }
        }

        [Benchmark]
        public string fake_expressionManuallyConstruted()
        {
            var sb = new StringBuilder();
            dslRoutineExpressionManuallyConstruted(sb, box);
            var json = sb.ToString();
            return json;
        }


        [Benchmark]
        public string fake_delegateManuallyConstruted()
        {
            var sb = new StringBuilder();
            dslRoutineDelegateManuallyConstrutedFormatter(sb, box);
            var json = sb.ToString();
            return json;
        }

        //[Benchmark]
        //public string JsonNet_Default()
        //{
        //    string text = JsonConvert.SerializeObject(box);
        //    return text;
        //}


        //[Benchmark]
        //public string JsonNet_Indented()
        //{
        //    string text = JsonConvert.SerializeObject(
        //        box, Formatting.Indented,
        //        new Newtonsoft.Json.JsonSerializerSettings { });
        //    return text;
        //}

        //[Benchmark]
        //public string JsonNet_NullIgnore()
        //{
        //    string text = JsonConvert.SerializeObject(
        //        box,
        //        new Newtonsoft.Json.JsonSerializerSettings
        //        {
        //            NullValueHandling = NullValueHandling.Ignore
        //        });
        //    return text;
        //}

        //[Benchmark]
        //public string JsonNet_DateFormatFF()
        //{
        //    string text = JsonConvert.SerializeObject(
        //        box,
        //        new Newtonsoft.Json.JsonSerializerSettings
        //        {
        //            DateFormatString = "yyyy-MM-ddTHH:mm:ssK"
        //        });
        //    return text;
        //}

        //[Benchmark]
        //public string JsonNet_DateFormatSS()
        //{
        //    string text = JsonConvert.SerializeObject(
        //        box,
        //        new Newtonsoft.Json.JsonSerializerSettings
        //        {
        //            DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK"
        //        });
        //    return text;
        //}

        //[Benchmark]
        //public string ServiceStack_SerializeToString()
        //{
        //    var json = ServiceStack.Text.JsonSerializer.SerializeToString(box);
        //    return json;
        //}
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