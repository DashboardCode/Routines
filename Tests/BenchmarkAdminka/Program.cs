using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace BenchmarkAdminka
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkAdminkaRoutine>();
        }
    }

    //public class MyManualConfig : ManualConfig
    //{
    //    public MyManualConfig()
    //    {
    //        Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp20));
    //        Add(Job.Default.With(CsProjClassicNetToolchain.Net47));
    //    }
    //}
}
