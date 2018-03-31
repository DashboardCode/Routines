``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


```
|                                   Method |     Job | Runtime |     Toolchain |        Mean |       Error |        StdDev |      Median |         Min |          Max | Rank |    Gen 0 | Allocated |
|----------------------------------------- |-------- |-------- |-------------- |------------:|------------:|--------------:|------------:|------------:|-------------:|-----:|---------:|----------:|
|                    MeasureRoutineLogList | Default |    Core | .NET Core 2.0 | 4,740.53 us |  89.3061 us |    79.1675 us | 4,736.57 us | 4,624.52 us |  4,925.01 us |    3 |  93.7500 | 290.61 KB |
|     MeasureRoutineNoAuthorizationLogList | Default |    Core | .NET Core 2.0 |    12.60 us |   0.1937 us |     0.1812 us |    12.62 us |    12.33 us |     12.88 us |    1 |   2.1667 |    6.7 KB |
|          MeasureRoutineRepositoryLogList | Default |    Core | .NET Core 2.0 | 5,399.80 us | 106.4799 us |   165.7763 us | 5,386.00 us | 5,180.85 us |  5,813.78 us |    5 | 101.5625 | 323.07 KB |
| MeasureRoutineRepositoryExceptionLogList | Default |    Core | .NET Core 2.0 | 8,469.62 us | 168.2959 us |   332.1998 us | 8,488.46 us | 7,822.17 us |  9,503.34 us |    9 | 140.6250 | 443.25 KB |
|     MeasureRoutineRepositoryErrorLogList | Default |    Core | .NET Core 2.0 | 6,987.52 us | 139.5641 us |   398.1843 us | 6,903.78 us | 6,364.13 us |  7,951.75 us |    7 | 140.6250 | 431.98 KB |
|                    MeasureRoutineLogList |     Clr |     Clr |       Default | 4,984.89 us | 118.5690 us |   332.4802 us | 4,900.25 us | 4,476.66 us |  5,810.10 us |    4 |  93.7500 |  292.7 KB |
|     MeasureRoutineNoAuthorizationLogList |     Clr |     Clr |       Default |    27.67 us |   0.4979 us |     0.4657 us |    27.51 us |    27.14 us |     28.69 us |    2 |   3.2959 |  10.18 KB |
|          MeasureRoutineRepositoryLogList |     Clr |     Clr |       Default | 5,687.06 us | 112.8754 us |   217.4727 us | 5,664.34 us | 5,384.49 us |  6,352.38 us |    6 | 101.5625 |  327.2 KB |
| MeasureRoutineRepositoryExceptionLogList |     Clr |     Clr |       Default | 7,148.80 us | 375.8163 us | 1,084.3159 us | 6,786.87 us | 5,979.00 us | 10,254.23 us |    7 | 125.0000 | 397.97 KB |
|     MeasureRoutineRepositoryErrorLogList |     Clr |     Clr |       Default | 7,820.02 us | 160.6452 us |   305.6441 us | 7,724.83 us | 7,464.86 us |  8,692.80 us |    8 | 156.2500 |  527.9 KB |
