``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


```
|                                Method |     Job | Runtime |     Toolchain |         Mean |       Error |        StdDev |          Min |          Max |       Median | Rank |    Gen 0 |    Gen 1 | Allocated |
|-------------------------------------- |-------- |-------- |-------------- |-------------:|------------:|--------------:|-------------:|-------------:|-------------:|-----:|---------:|---------:|----------:|
|                    MeasureRoutineNLog | Default |    Core | .NET Core 2.0 |           NA |          NA |            NA |           NA |           NA |           NA |    ? |      N/A |      N/A |       N/A |
|     MeasureRoutineNoAuthorizationNLog | Default |    Core | .NET Core 2.0 |     29.10 us |   0.5776 us |     0.6180 us |     28.22 us |     30.67 us |     28.95 us |    1 |   3.8147 |        - |   12089 B |
|          MeasureRoutineRepositoryNLog | Default |    Core | .NET Core 2.0 |  6,533.13 us | 127.7631 us |   131.2033 us |  6,278.30 us |  6,828.75 us |  6,505.27 us |    5 | 179.6875 |        - |  587770 B |
| MeasureRoutineRepositoryExceptionNLog | Default |    Core | .NET Core 2.0 |  9,071.48 us | 180.0266 us |   184.8740 us |  8,776.26 us |  9,459.24 us |  9,059.94 us |    6 | 203.1250 |        - |  665821 B |
|                    MeasureRoutineNLog |     Clr |     Clr |       Default | 25,964.08 us | 518.0102 us | 1,241.1193 us | 23,859.87 us | 29,295.83 us | 25,701.05 us |    7 | 406.2500 | 156.2500 | 2015429 B |
|     MeasureRoutineNoAuthorizationNLog |     Clr |     Clr |       Default |     38.53 us |   0.3485 us |     0.2910 us |     38.08 us |     39.06 us |     38.60 us |    2 |   5.5542 |        - |   17567 B |
|          MeasureRoutineRepositoryNLog |     Clr |     Clr |       Default |  6,132.10 us |  63.5267 us |    53.0477 us |  6,043.90 us |  6,237.45 us |  6,130.68 us |    3 | 187.5000 |   7.8125 |  612070 B |
| MeasureRoutineRepositoryExceptionNLog |     Clr |     Clr |       Default |  6,232.87 us | 123.7313 us |    96.6013 us |  6,107.25 us |  6,339.87 us |  6,263.40 us |    4 | 195.3125 |        - |  633272 B |

Benchmarks with issues:
  BenchmarkAdminkaRoutineNLogLogger.MeasureRoutineNLog: Job-GGHVXY(Runtime=Core, Toolchain=.NET Core 2.0)
