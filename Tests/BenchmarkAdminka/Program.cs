using BenchmarkDotNet.Running;

namespace BenchmarkAdminka
{
    class Program
    {
        // TODO: adjust number of tests to avoid gygabyte logs
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