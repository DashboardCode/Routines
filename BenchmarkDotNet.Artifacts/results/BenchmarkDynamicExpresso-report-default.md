
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
.NET Core SDK=2.1.100
  [Host]     : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Job-VVJUHZ : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


              Method |     Job | Runtime |     Toolchain |     Mean |    Error |    StdDev |      Min |      Max |   Median | Rank |   Gen 0 |  Gen 1 | Allocated |
-------------------- |-------- |-------- |-------------- |---------:|---------:|----------:|---------:|---------:|---------:|-----:|--------:|-------:|----------:|
 TestEvalArithmetics | Default |    Core | .NET Core 2.0 | 404.9 us | 2.460 us |  2.181 us | 402.3 us | 410.7 us | 404.8 us |    3 | 16.6016 |      - |   51.7 KB |
     TestEvalCompile | Default |    Core | .NET Core 2.0 | 482.7 us | 8.711 us |  8.148 us | 473.2 us | 501.3 us | 480.0 us |    4 | 12.6953 | 0.4883 |  40.36 KB |
 TestEvalArithmetics |     Clr |     Clr |       Default | 354.9 us | 7.071 us |  7.261 us | 346.5 us | 370.8 us | 351.6 us |    1 | 18.5547 |      - |  57.86 KB |
     TestEvalCompile |     Clr |     Clr |       Default | 392.3 us | 7.711 us | 10.294 us | 378.4 us | 409.5 us | 388.9 us |    2 | 14.1602 | 0.4883 |  43.92 KB |
