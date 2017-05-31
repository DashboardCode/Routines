using BenchmarkDotNet.Running;

namespace BenchmarkClassic
{
    class Program
    {
        static void Main(string[] args)
        {
            var b = new BenchmarkJson();
            
            var t1 = b.JsonNet();
            var t2 = b.RoutineExpressionCompiled();
            var t3 = b.ServiceStack1();
            BenchmarkRunner.Run<BenchmarkJson>();
        }
    }
}
