using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new BenchmarkForEach();
            //var x1 = b.DelegateBuilded();
            //var x2 = b.Expression();
            //var s1 = b.();
            //var s2 = b.StringFloat();

            //var j1 = b.RoutineConstructed();
            //var j2 = b.RoutineExpressionCompiled();
            //var j3 = b.RoutineFunc();
            //var j4 = b.JsonNet();

            //var j5 = b.RoutineSerializerFunc();
            //var j6 = b.RoutineSerializerBuilded();

            //var jO = b.TestFuncDynamicInvoke();
            //var jA = b.TestFunc();
            //var jB = b.TestFuncBuilded();
            BenchmarkRunner.Run<BenchmarkForEach>();
        }
    }
}