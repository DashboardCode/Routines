using System;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    //[Config(typeof(ManualWindowsDiagnosersConfig))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
#if !NETCOREAPP2_0
    //[HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.BranchMispredictions, BenchmarkDotNet.Diagnosers.HardwareCounter.BranchInstructions)]
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    [BenchmarkDotNet.Attributes.Jobs.RyuJitX64Job]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.InliningDiagnoser]
#endif
    public class BenchmarkCallSimpleDisasm
    {
        static Func<StringBuilder, int, int, bool> callLambda;
        static Func<StringBuilder, int, int, bool> callLambdaConst;
        static Func<StringBuilder, int, int, bool> callBuilded;
        static Func<StringBuilder, int, int, bool> callBuildedReal;
        private static bool Append<T>(StringBuilder sb, T i1, T i2, Func<T, T, T> operation)
        {
            sb.Append(operation(i1, i2));
            return true;
        }

        private static Func<StringBuilder, T, T, bool> BuildCallMethod<T>(Func<T, T, T> operation)
        {
            return (sb, i1, i2) => { sb.Append(operation(i1, i2)); return true; };
        }

        private static int AddMethod(int a, int b)
        {
            return a + b;
        }

        static BenchmarkCallSimpleDisasm()
        {
            var x = Expression.Parameter(typeof(int));
            var y = Expression.Parameter(typeof(int));
            var additionExpr = Expression.Add(x, y);

            #region add lambda
            addLambda = (a, b) => a + b;
            addLambdaConst = AddMethod;

            addBuilded =
                          Expression.Lambda<Func<int, int, int>>(
                              additionExpr, x, y).Compile();
            #endregion

            #region call lambda
            callLambdaConst = BuildCallMethod<int>(AddMethod);
            callLambda = BuildCallMethod<int>((a, b) => a + b);

            var operationDelegate = Expression.Lambda<Func<int, int, int>>(additionExpr, x, y).Compile();
            callBuilded = BuildCallMethod(operationDelegate);

            var operationExpressionConst = Expression.Constant(operationDelegate, operationDelegate.GetType());

            var sb1 = Expression.Parameter(typeof(StringBuilder), "sb");
            var i1 = Expression.Parameter(typeof(int), "i1");
            var i2 = Expression.Parameter(typeof(int), "i2");
            var appendMethodInfo = typeof(BenchmarkCallSimple).GetTypeInfo().GetDeclaredMethod(nameof(BenchmarkCallSimpleDisasm.Append));
            var appendMethodInfoGeneric = appendMethodInfo.MakeGenericMethod(typeof(int));
            var appendCallExpression = Expression.Call(appendMethodInfoGeneric,
                    new Expression[] { sb1, i1, i2, operationExpressionConst }
                );
            var appendLambda = Expression.Lambda(appendCallExpression, new[] { sb1, i1, i2 });
            callBuildedReal = (Func<StringBuilder, int, int, bool>)(appendLambda.Compile());
            #endregion

            #region Save Lambda
#if NET47
            // some 
            var assemblyBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("testLambda"),System.Reflection.Emit.AssemblyBuilderAccess.Save);
            var modelBuilder = assemblyBuilder.DefineDynamicModule("testLambda_module", "testLambda.dll");
            var typeBuilder = modelBuilder.DefineType("testLambda_type");
            var method = typeBuilder.DefineMethod("testLambda_method", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), 
                new[] { typeof(StringBuilder), typeof(int), typeof(int) });
            appendLambda.CompileToMethod(method);
            typeBuilder.CreateType();
            assemblyBuilder.Save("testLambda.dll");
#else
            //var assemblyBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("testLambda"),
            //System.Reflection.Emit.AssemblyBuilderAccess.Run);

            //var modelBuilder = assemblyBuilder.DefineDynamicModule("testLambda_module");
            //var typeBuilder = modelBuilder.DefineType("testLambda_type");
            //var method = typeBuilder.DefineMethod("testLambda_method", MethodAttributes.Public | MethodAttributes.Static, typeof(bool),
            //    new[] { typeof(StringBuilder), typeof(int), typeof(int) });

            //// Core doesn't containt CompileToMethod and Save
            //appendLambda.CompileToMethod(method);
            //assemblyBuilder.Save("testLambda.dll");
#endif
            #endregion
        }

#region Call lambda
        [Benchmark]
        public string CallBuildedReal()
        {
            StringBuilder sb = new StringBuilder();
            var b = callBuildedReal(sb, 1, 2);
            return sb.ToString();
        }

        [Benchmark]
        public string CallBuilded()
        {
            StringBuilder sb = new StringBuilder();
            var b = callBuilded(sb, 1, 2);
            return sb.ToString();
        }

        [Benchmark]
        public string CallLambda()
        {
            StringBuilder sb = new StringBuilder();
            var b = callLambda(sb, 1, 2);
            return sb.ToString();
        }

        [Benchmark]
        public string CallLambdaConst()
        {
            StringBuilder sb = new StringBuilder();
            var b = callLambdaConst(sb, 1, 2);
            return sb.ToString();
        }
#endregion

#region Add lambda
        static Func<int, int, int> addLambda;
        static Func<int, int, int> addLambdaConst;
        static Func<int, int, int> addBuilded;

        //[Benchmark]
        public int AddBuilded()
        {
            return addBuilded(1, 2);
        }

        //[Benchmark]
        public int AddLambda()
        {
            return addLambda(1, 2);
        }

        //[Benchmark]
        public int AddLambdaConst()
        {
            return addLambdaConst(1, 2);
        }
#endregion
    }
}