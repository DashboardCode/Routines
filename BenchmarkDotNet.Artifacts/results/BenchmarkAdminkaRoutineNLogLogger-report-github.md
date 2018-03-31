``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


```
|                                    Method |     Job | Runtime |     Toolchain |        Mean |     Error |     StdDev |      Median |         Min |         Max | Rank |    Gen 0 |   Gen 1 |  Allocated |
|------------------------------------------ |-------- |-------- |-------------- |------------:|----------:|-----------:|------------:|------------:|------------:|-----:|---------:|--------:|-----------:|
|                        MeasureRoutineNLog | Default |    Core | .NET Core 2.0 |  7,844.5 us | 293.71 us |   838.0 us |  7,573.8 us |  6,553.5 us | 10,018.0 us |    3 | 109.3750 |       - |  356.25 KB |
|         MeasureRoutineNoAuthorizationNLog | Default |    Core | .NET Core 2.0 |    669.6 us |  34.41 us |   101.5 us |    676.0 us |    474.2 us |    884.1 us |    1 |  12.6953 |       - |   40.44 KB |
|              MeasureRoutineRepositoryNLog | Default |    Core | .NET Core 2.0 |  7,750.8 us | 154.26 us |   437.6 us |  7,651.4 us |  6,974.8 us |  8,857.5 us |    3 | 125.0000 |       - |  390.49 KB |
|     MeasureRoutineRepositoryExceptionNLog | Default |    Core | .NET Core 2.0 | 11,943.4 us | 232.32 us |   318.0 us | 11,943.2 us | 11,495.7 us | 12,588.5 us |    5 | 187.5000 |       - |  941.91 KB |
| MeasureRoutineRepositoryExceptionMailNLog | Default |    Core | .NET Core 2.0 | 40,017.9 us | 800.27 us | 1,917.4 us | 39,531.1 us | 37,489.5 us | 45,592.0 us |   10 | 250.0000 |       - |  779.86 KB |
|         MeasureRoutineRepositoryErrorNLog | Default |    Core | .NET Core 2.0 | 10,713.6 us | 208.78 us |   365.7 us | 10,619.4 us | 10,239.4 us | 11,678.6 us |    4 | 187.5000 | 46.8750 |  932.98 KB |
|                        MeasureRoutineNLog |     Clr |     Clr |       Default | 14,276.4 us | 282.52 us |   431.4 us | 14,114.8 us | 13,776.6 us | 15,191.6 us |    6 | 109.3750 |       - |   358.9 KB |
|         MeasureRoutineNoAuthorizationNLog |     Clr |     Clr |       Default |  4,631.3 us |  91.78 us |   197.6 us |  4,573.7 us |  4,390.1 us |  5,151.0 us |    2 |   7.8125 |       - |   43.48 KB |
|              MeasureRoutineRepositoryNLog |     Clr |     Clr |       Default | 15,071.7 us | 280.15 us |   275.1 us | 15,084.5 us | 14,642.9 us | 15,664.0 us |    7 | 125.0000 |       - |  394.33 KB |
|     MeasureRoutineRepositoryExceptionNLog |     Clr |     Clr |       Default | 17,275.6 us | 341.65 us |   319.6 us | 17,260.8 us | 16,674.3 us | 17,935.0 us |    8 | 281.2500 | 93.7500 |  879.68 KB |
| MeasureRoutineRepositoryExceptionMailNLog |     Clr |     Clr |       Default | 45,434.7 us | 875.36 us | 1,107.0 us | 45,286.5 us | 43,712.2 us | 48,147.7 us |   11 | 187.5000 |       - |  756.17 KB |
|         MeasureRoutineRepositoryErrorNLog |     Clr |     Clr |       Default | 18,157.7 us | 361.19 us |   623.0 us | 17,970.7 us | 17,354.9 us | 19,998.1 us |    9 | 312.5000 | 93.7500 | 1022.85 KB |
