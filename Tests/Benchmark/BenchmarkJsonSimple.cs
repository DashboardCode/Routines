using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using DashboardCode.Routines;
using DashboardCode.Routines.Json;

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
        static Func<Box2, string> formatter;
        static Func<StringBuilder, Box2, bool> serializerFuncCompiled;
        static Func<StringBuilder, Box2, bool> serializerFunc;
        //static NExpJsonSerializer<Box2> serializer3;
        static BenchmarkJsonSimple()
        {
            box = new Box2() {
                B1 = true
            };

            Include<Box2> include = (i) => i.Include(e => e.B1);
            var include2 = include.AppendLeafs(); 

            var process = new ChainVisitor<Box2>();
            var chain = new Chain<Box2>(process);
            include2.Invoke(chain);
            var serializerNode = process.Root;

            formatter = JsonChainManager.ComposeFormatter<Box2>(serializerNode.ComposeInclude<Box2>(), stringBuilderCapacity: 4000);

            serializerFunc = (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeObject(sbP, tP,
                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", o => o.B1, JsonValueStringBuilderExtensions.SerializeBool)
                    );

            Expression<Func<StringBuilder, Box2, bool>> serializerFuncCompiledExp = (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeObject(sbP, tP,
                       (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", o => o.B1, JsonValueStringBuilderExtensions.SerializeBool)
                   );
            serializerFuncCompiled = serializerFuncCompiledExp.Compile();

            Include<Box2> includeAlt = (i) => i.Include(e => e.B1);
            //serializer3 = includeAlt.BuildNExpJsonSerializer();
            Func<Box2, bool> getterDelegate = o => o.B1;
            Expression<Func<StringBuilder, Box2, bool>> testFuncBuilded2Exp = (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", getterDelegate, JsonValueStringBuilderExtensions.SerializeBool);
            testFuncBuilded2 = testFuncBuilded2Exp.Compile();

            testFunc = (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeStructProperty(sb4, t4, "B1", getterDelegate, JsonValueStringBuilderExtensions.SerializeBool);

            #region Test
            var getterDelegateType = typeof(Func<,>).MakeGenericType(typeof(Box2), typeof(bool));
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegateType);
            //var oParameterExpression = Expression.Parameter(typeof(Box2), "o");
            //var getterMemberExpression = Expression.Property(oParameterExpression, typeof(Box2).GetTypeInfo().GetDeclaredProperty("B1"));
            //var getterExpression = Expression.Lambda(getterMemberExpression, new[] { oParameterExpression });
            //var getterDelegate = getterExpression.Compile(); // Func<T, TProp> getter
            //var getterConstantExpression = Expression.Constant(getterDelegate);

            var formatterMethodInfo   = typeof(JsonValueStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonValueStringBuilderExtensions.SerializeBool));
            var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), typeof(bool), typeof(bool));
            var formatterDelegate     = formatterMethodInfo.CreateDelegate(formatterDelegateType);
            var formatterConstantExpression = Expression.Constant(formatterDelegate, formatterDelegateType);

            var serializePropertyMethodInfo = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeStructProperty));
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
            var json = formatter(box);
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
        //public string RoutineInterpretated()
        //{
        //    var text = serializer3.Serialize(box);
        //    return text;
        //}

        //[Benchmark]
        public string JsonNet()
        {
            string text = JsonConvert.SerializeObject(
                box,
                new Newtonsoft.Json.JsonSerializerSettings
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
