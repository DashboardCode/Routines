
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


                                    Method |     Job | Runtime |     Toolchain |        Mean |     Error |      StdDev |      Median |         Min |         Max | Rank |    Gen 0 |   Gen 1 | Allocated |
------------------------------------------ |-------- |-------- |-------------- |------------:|----------:|------------:|------------:|------------:|------------:|-----:|---------:|--------:|----------:|
                        MeasureRoutineNLog | Default |    Core | .NET Core 2.0 |  7,064.8 us | 139.54 us |   303.36 us |  6,995.6 us |  6,602.6 us |  8,059.9 us |    3 | 109.3750 |       - |  363898 B |
         MeasureRoutineNoAuthorizationNLog | Default |    Core | .NET Core 2.0 |    669.5 us |  26.60 us |    77.60 us |    666.1 us |    532.3 us |    873.2 us |    1 |  12.6953 |       - |   41732 B |
              MeasureRoutineRepositoryNLog | Default |    Core | .NET Core 2.0 |  8,840.4 us | 374.65 us | 1,068.91 us |  8,546.1 us |  7,547.0 us | 11,662.0 us |    4 | 125.0000 |       - |  399698 B |
     MeasureRoutineRepositoryExceptionNLog | Default |    Core | .NET Core 2.0 | 13,615.7 us | 291.47 us |   836.28 us | 13,385.0 us | 12,357.3 us | 16,068.4 us |    6 | 187.5000 | 78.1250 |  962227 B |
 MeasureRoutineRepositoryExceptionMailNLog | Default |    Core | .NET Core 2.0 |          NA |        NA |          NA |          NA |          NA |          NA |    ? |      N/A |     N/A |       N/A |
         MeasureRoutineRepositoryErrorNLog | Default |    Core | .NET Core 2.0 | 12,762.0 us | 262.26 us |   743.99 us | 12,596.4 us | 11,592.0 us | 14,919.5 us |    5 | 218.7500 | 78.1250 |  959307 B |
                        MeasureRoutineNLog |     Clr |     Clr |       Default | 16,236.3 us | 332.76 us |   694.58 us | 16,163.5 us | 15,167.4 us | 18,251.4 us |    7 | 109.3750 |       - |  367888 B |
         MeasureRoutineNoAuthorizationNLog |     Clr |     Clr |       Default |  4,944.1 us |  96.92 us |   169.75 us |  4,943.7 us |  4,641.2 us |  5,253.5 us |    2 |   7.8125 |       - |   44697 B |
              MeasureRoutineRepositoryNLog |     Clr |     Clr |       Default | 15,977.4 us | 318.17 us |   849.25 us | 15,874.1 us | 14,566.5 us | 18,461.4 us |    7 | 125.0000 |       - |  405560 B |
     MeasureRoutineRepositoryExceptionNLog |     Clr |     Clr |       Default | 19,565.8 us | 681.18 us | 1,943.45 us | 18,935.4 us | 17,292.9 us | 25,125.0 us |    8 | 281.2500 | 62.5000 |  902876 B |
 MeasureRoutineRepositoryExceptionMailNLog |     Clr |     Clr |       Default |          NA |        NA |          NA |          NA |          NA |          NA |    ? |      N/A |     N/A |       N/A |
         MeasureRoutineRepositoryErrorNLog |     Clr |     Clr |       Default | 19,977.6 us | 712.12 us | 2,043.22 us | 19,305.7 us | 17,586.2 us | 25,268.6 us |    8 | 312.5000 | 62.5000 | 1051718 B |

Benchmarks with issues:
  BenchmarkAdminkaRoutineNLogLogger.MeasureRoutineRepositoryExceptionMailNLog: Job-GGHVXY(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkAdminkaRoutineNLogLogger.MeasureRoutineRepositoryExceptionMailNLog: Clr(Runtime=Clr)
