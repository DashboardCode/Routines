using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;

namespace BenchmarkAdminka
{
    public class MultipleRuntimesManualConfig : ManualConfig
    {
        public MultipleRuntimesManualConfig()
        {
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core31));
            AddJob(Job.Default.WithRuntime(ClrRuntime.Net48));

            //read this: https://github.com/dotnet/BenchmarkDotNet/issues/697
            //Add(Job.Default.With(CsProjClassicNetToolchain.Net47));
            //Add(Job.Clr.With(CsProjClassicNetToolchain.Net47));
        }
    }
}