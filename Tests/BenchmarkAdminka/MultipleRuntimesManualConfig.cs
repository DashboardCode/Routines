using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace BenchmarkAdminka
{
    public class MultipleRuntimesManualConfig : ManualConfig
    {
        public MultipleRuntimesManualConfig()
        {
            Add(Job.Core.With(CsProjCoreToolchain.NetCoreApp20));
            Add(Job.Clr);

            //read this: https://github.com/dotnet/BenchmarkDotNet/issues/697
            //Add(Job.Default.With(CsProjClassicNetToolchain.Net47));
            //Add(Job.Clr.With(CsProjClassicNetToolchain.Net47));
        }
    }
}