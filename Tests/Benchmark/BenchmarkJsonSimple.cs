using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Vse.Routines;
using Vse.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob /*, CoreJob*/]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser /*, InliningDiagnoser*/]
    public class BenchmarkJsonSimple
    {
        static Box2 box;
        static Func<StringBuilder, Box2, bool> serializerBuilded;
        static Func<StringBuilder, Box2, bool> serializerFuncCompiled;
        static Func<StringBuilder, Box2, bool> serializerFunc;
        static NExpJsonSerializer<Box2> serializer3;
        static BenchmarkJsonSimple()
        {
            box = new Box2() {
                B1 = true
            };

            var parser = new SerializerNExpParser<Box2>();
            var includable = new Includable<Box2>(parser);
            Include<Box2> include = (i) => i.Include(e=>e.B1);
            include.Invoke(includable);

            parser.Root.AppendLeafs();
            var serializerNode = parser.Root;

            serializerBuilded = NExpJsonSerializerTools.BuildSerializer<Box2>(serializerNode);

            serializerFunc = (sbP, tP) => NExpJsonSerializerStringBuilderExtensions.SerializeObjectNotEmpty(sbP, tP,
                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", o => o.B1, NExpJsonSerializerStringBuilderExtensions.SerializeBool)
                    );

            serializerFuncCompiled = NExpJsonSerializerTools.ExpressionBuilder<Box2>(
                    (sbP, tP) => NExpJsonSerializerStringBuilderExtensions.SerializeObjectNotEmpty(sbP, tP,
                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", o => o.B1, NExpJsonSerializerStringBuilderExtensions.SerializeBool)
                    )
                );

            Include<Box2> includeAlt = (i) => i.Include(e => e.B1);
            serializer3 = includeAlt.BuildNExpJsonSerializer();
            Func<Box2, bool> getterDelegate = o => o.B1;
            testFuncBuilded2 = NExpJsonSerializerTools.ExpressionBuilder<Box2, bool>(
                        (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", getterDelegate, NExpJsonSerializerStringBuilderExtensions.SerializeBool)
                );

            testFunc = (sb4, t4) => NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", getterDelegate, NExpJsonSerializerStringBuilderExtensions.SerializeBool);

            #region Test
            var getterDelegateType = typeof(Func<,>).MakeGenericType(typeof(Box2), typeof(bool));
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegateType);
            //var oParameterExpression = Expression.Parameter(typeof(Box2), "o");
            //var getterMemberExpression = Expression.Property(oParameterExpression, typeof(Box2).GetTypeInfo().GetDeclaredProperty("B1"));
            //var getterExpression = Expression.Lambda(getterMemberExpression, new[] { oParameterExpression });
            //var getterDelegate = getterExpression.Compile(); // Func<T, TProp> getter
            //var getterConstantExpression = Expression.Constant(getterDelegate);

            var formatterMethodInfo   = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeBool));
            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), typeof(bool), typeof(bool));
            var formatterDelegate     = formatterMethodInfo.CreateDelegate(formatterDelegateType);
            var formatterConstantExpression = Expression.Constant(formatterDelegate, formatterDelegateType);

            var serializePropertyMethodInfo = typeof(NExpJsonSerializerStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(NExpJsonSerializerStringBuilderExtensions.SerializeStructProperty));
            var serializePropertyGenericMethodInfo = serializePropertyMethodInfo.MakeGenericMethod(typeof(Box2), typeof(bool));

            var serializePropertyDelegateType = typeof(Func<,,,,,>).MakeGenericType(typeof(StringBuilder), typeof(string), typeof(Box2), typeof(Func<Box2, bool>), typeof(Func<StringBuilder, bool, bool>), typeof(bool));
            var serializePropertyDelegate = serializePropertyGenericMethodInfo.CreateDelegate(serializePropertyDelegateType);

            testFuncDynamicInvoke = (sbx, tx) => { return (bool)(serializePropertyDelegate.DynamicInvoke(sbx, "B1", tx, getterDelegate, formatterDelegate)); };
            
            var serializationNameConstant = Expression.Constant("B1", typeof(string));
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(typeof(Box2), "t");

            var serializePropertyMethodCallExpression = Expression.Call(serializePropertyGenericMethodInfo,
                new Expression[] {sb, t, serializationNameConstant, getterConstantExpression, formatterConstantExpression}
            );

            var serializePropertyLambda = Expression.Lambda(serializePropertyMethodCallExpression, new[] { sb, t });
            testFuncBuilded = (Func<StringBuilder, Box2, bool>)serializePropertyLambda.Compile();
            #endregion
        }
        static Func<StringBuilder, Box2, bool> testFuncBuilded;
        static Func<StringBuilder, Box2, bool> testFuncBuilded2;
        static Func<StringBuilder, Box2, bool> testFunc;
        static Func<StringBuilder, Box2, bool> testFuncDynamicInvoke;

        [Benchmark]
        public string TestFunc()
        {
            var sb = new StringBuilder(4000);
            testFunc(sb, box);
            var json = sb.ToString();
            return json;
        }

        [Benchmark]
        public string TestFuncBuilded()
        {
            var sb = new StringBuilder(4000);
            testFuncBuilded(sb, box);
            var json = sb.ToString();
            return json;
        }

        [Benchmark]
        public string TestFuncDynamicInvoke()
        {
            var sb = new StringBuilder(4000);
            testFuncDynamicInvoke(sb, box);
            var json = sb.ToString();
            return json;
        }

        ////[Benchmark]
        public string RoutineSerializerBuilded()
        {
            var sb = new StringBuilder(4000);
            serializerBuilded(sb, box);
            var json = sb.ToString();
            return json;
        }

        //////[Benchmark]
        public string RoutineSerializerFunc()
        {
            var sb = new StringBuilder(4000);
            serializerFunc(sb, box);
            var json = sb.ToString();
            return json;
        }

        //[Benchmark]
        public string RoutineSerializerFuncCompiled()
        {
            var sb = new StringBuilder(4000);
            serializerFuncCompiled(sb, box);
            var json = sb.ToString();
            return json;
        }

        //[Benchmark]
        public string RoutineInterpretated()
        {
            var text = serializer3.Serialize(box);
            return text;
        }

        //[Benchmark]
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

    public class Box2
    {
        public bool B1 { get; set; }
    }

    //public class Row2
    //{
    //    public DateTime At { get; set; }
    //    public int I1 { get; set; }
    //    public int? I2 { get; set; }
    //    public bool B1 { get; set; }
    //    public bool? B2 { get; set; }
    //    public decimal D1 { get; set; }
    //    public decimal D2 { get; set; }
    //    public decimal D3 { get; set; }
    //    public decimal? D4 { get; set; }
    //    public double F1 { get; set; }
    //    public double F2 { get; set; }
    //    public double F3 { get; set; }
    //    public double? F4 { get; set; }
    //}
}
