using System;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    //[Config(typeof(ManualWindowsDiagnosersConfig))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
#if !(NETCOREAPP)
    //[HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.BranchMispredictions, BenchmarkDotNet.Diagnosers.HardwareCounter.BranchInstructions)]
    [DisassemblyDiagnoser(printSource: true)]
    [RyuJitX64Job]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.InliningDiagnoser(true,true)]
#endif
    public class BenchmarkCallSimpleDisasm
    {
        readonly static Func<StringBuilder, int, int, bool> callLambda;
        readonly static Func<StringBuilder, int, int, bool> callLambdaConst;
        readonly static Func<StringBuilder, int, int, bool> callBuilded;
        readonly static Func<StringBuilder, int, int, bool> callBuildedReal;
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

#pragma warning disable CA1810 // Initialize reference type static fields inline
        static BenchmarkCallSimpleDisasm()
#pragma warning restore CA1810 // Initialize reference type static fields inline
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
#if NET48
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
#pragma warning disable CA1822 // Mark members as static
        public string CallBuildedReal()
#pragma warning restore CA1822 // Mark members as static
        {
            StringBuilder sb = new StringBuilder();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var b = callBuildedReal(sb, 1, 2);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            return sb.ToString();
        }

        [Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public string CallBuilded()
#pragma warning restore CA1822 // Mark members as static
        {
            StringBuilder sb = new StringBuilder();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var b = callBuilded(sb, 1, 2);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            return sb.ToString();
        }

        [Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public string CallLambda()
#pragma warning restore CA1822 // Mark members as static
        {
            StringBuilder sb = new StringBuilder();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var b = callLambda(sb, 1, 2);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            return sb.ToString();
        }

        [Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public string CallLambdaConst()
#pragma warning restore CA1822 // Mark members as static
        {
            StringBuilder sb = new StringBuilder();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            var b = callLambdaConst(sb, 1, 2);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            return sb.ToString();
        }
        #endregion

        #region Add lambda
        readonly static Func<int, int, int> addLambda;
        readonly static Func<int, int, int> addLambdaConst;
        readonly static Func<int, int, int> addBuilded;

        //[Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public int AddBuilded()
#pragma warning restore CA1822 // Mark members as static
        {
            return addBuilded(1, 2);
        }

        //[Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public int AddLambda()
#pragma warning restore CA1822 // Mark members as static
        {
            return addLambda(1, 2);
        }

        //[Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public int AddLambdaConst()
#pragma warning restore CA1822 // Mark members as static
        {
            return addLambdaConst(1, 2);
        }
#endregion
    }
}