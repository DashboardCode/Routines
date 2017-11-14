using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new BenchmarkConcurencyCollection();
            b.ConcurrentBagAdd();
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
            //BenchmarkRunner.Run<BenchmarkConcurencyCollection>();
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