using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //var b = new BenchmarkAsyncNotAwaitInterface();
            //var i1 = b.Completed();
            //var i2 = b.CompletedAwait();
            //var i3 = b.Pragma();
            //var i4 = b.Yield();
            //b.ConcurrentBagAdd();
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
            BenchmarkRunner.Run<BenchmarkStructOrClass>();
        }
    }

    public class MyManualConfig : ManualConfig
    {
        public MyManualConfig()
        {
            Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp20)); 
            Add(Job.Default.With(CsProjClassicNetToolchain.Net47));
        }
    }
}