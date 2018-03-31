using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace BenchmarkClassic
{
    class Program
    {
        static void Main(string[] args) =>
            BenchmarkRunner.Run<BenchmarkConverAll>();
    }

    public class MyManualConfig : ManualConfig
    {
        public MyManualConfig()
        {
            
            Add(Job.Core.With(CsProjCoreToolchain.NetCoreApp20));
            //Add(Job.Clr);
        }
    }
}