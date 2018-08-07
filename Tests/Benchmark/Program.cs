using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkComposeFormatter>();
        }
    }

    public class CoreToolchain2JobConfig : ManualConfig
    {
        public CoreToolchain2JobConfig()
        {
            Add(Job.Core.With(CsProjCoreToolchain.NetCoreApp21));
            Add(Job.Clr.With(CsProjCoreToolchain.NetCoreApp21));
        }
    }

    /// <summary>
    /// Use this to utilize "Windows Only" BenchmarkDotNet diagnosers
    /// JIT Inlining Events(InliningDiagnoser);
    /// JIT Tail Call Events(TailCallDiagnoser);
    /// Hardware Counter Diagnoser;
    /// Disassembly Diagnoser(it can be utilized in BenchmarkDotNet core project but can be used only on Windows)
    /// So do not use it for performance comparisions with Core. [ClrJob] only!
    /// </summary>
    public class ManualWindowsDiagnosersConfig : ManualConfig
    {
        public ManualWindowsDiagnosersConfig()
        {
#if !NETCOREAPP
            //Add(Job.ShortRun.With(Jit.RyuJit).With(Platform.X64).With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp20));
            //Add(DisassemblyDiagnoser.Create(new DisassemblyDiagnoserConfig(printAsm: true, printPrologAndEpilog: true, recursiveDepth: 3)));
            //Add(new BenchmarkDotNet.Diagnostics.Windows.InliningDiagnoser());
#endif
        }
    }
}
