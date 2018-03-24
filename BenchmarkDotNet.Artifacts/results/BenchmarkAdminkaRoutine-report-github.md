``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


```
|                                   Method |     Job | Runtime |     Toolchain |        Mean |       Error |      StdDev |         Min |          Max |      Median | Rank |
|----------------------------------------- |-------- |-------- |-------------- |------------:|------------:|------------:|------------:|-------------:|------------:|-----:|
|                    MeasureRoutineLogList | Default |    Core | .NET Core 2.0 | 5,108.77 us |  74.9288 us |  66.4224 us | 4,984.59 us |  5,211.27 us | 5,095.07 us |    3 |
|     MeasureRoutineNoAuthorizationLogList | Default |    Core | .NET Core 2.0 |    27.24 us |   0.2750 us |   0.2297 us |    26.77 us |     27.58 us |    27.25 us |    1 |
|          MeasureRoutineRepositoryLogList | Default |    Core | .NET Core 2.0 | 5,138.93 us |  32.5367 us |  28.8429 us | 5,082.82 us |  5,194.55 us | 5,134.23 us |    3 |
| MeasureRoutineRepositoryExceptionLogList | Default |    Core | .NET Core 2.0 | 9,294.92 us | 194.7855 us | 451.4450 us | 8,345.45 us | 10,653.93 us | 9,307.33 us |    6 |
|                    MeasureRoutineLogList |     Clr |     Clr |       Default | 5,961.78 us | 114.3685 us | 122.3730 us | 5,801.58 us |  6,242.60 us | 5,974.48 us |    4 |
|     MeasureRoutineNoAuthorizationLogList |     Clr |     Clr |       Default |    38.26 us |   0.4310 us |   0.4032 us |    37.74 us |     39.08 us |    38.23 us |    2 |
|          MeasureRoutineRepositoryLogList |     Clr |     Clr |       Default | 6,004.13 us | 119.7606 us | 167.8876 us | 5,751.06 us |  6,364.04 us | 5,988.05 us |    4 |
| MeasureRoutineRepositoryExceptionLogList |     Clr |     Clr |       Default | 7,156.54 us | 140.6980 us | 206.2332 us | 6,912.33 us |  7,750.69 us | 7,059.30 us |    5 |
