
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
.NET Core SDK=2.1.100
  [Host]     : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Job-AVYIXW : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT


        Method |     Job | Runtime |     Toolchain |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |   Gen 0 |  Gen 1 | Allocated |
-------------- |-------- |-------- |-------------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|--------:|-------:|----------:|
 TestConverAll | Default |    Core | .NET Core 2.0 | 167.0 us | 0.6699 us | 0.6266 us | 166.3 us | 168.6 us | 167.0 us |    1 | 22.7051 | 0.2441 |   72064 B |
 TestConverAll |     Clr |     Clr |       Default |       NA |        NA |        NA |       NA |       NA |       NA |    ? |     N/A |    N/A |       N/A |

Benchmarks with issues:
  BenchmarkConverAll.TestConverAll: Clr(Runtime=Clr)
