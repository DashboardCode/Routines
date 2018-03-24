using BenchmarkDotNet.Running;

namespace BenchmarkClassic
{
    class Program
    {
        static void Main(string[] args) =>
            BenchmarkRunner.Run<BenchmarkConverAll>();
    }
}