
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


                                   Method |     Job | Runtime |     Toolchain |        Mean |       Error |      StdDev |         Min |         Max |      Median | Rank |    Gen 0 | Allocated |
----------------------------------------- |-------- |-------- |-------------- |------------:|------------:|------------:|------------:|------------:|------------:|-----:|---------:|----------:|
                    MeasureRoutineLogList | Default |    Core | .NET Core 2.0 | 5,538.32 us | 119.1932 us | 338.1312 us | 4,931.63 us | 6,428.13 us | 5,490.30 us |    4 |  93.7500 |  291.2 KB |
     MeasureRoutineNoAuthorizationLogList | Default |    Core | .NET Core 2.0 |    13.64 us |   0.1204 us |   0.1006 us |    13.46 us |    13.83 us |    13.63 us |    1 |   2.2583 |   6.98 KB |
          MeasureRoutineRepositoryLogList | Default |    Core | .NET Core 2.0 | 6,041.74 us | 132.3864 us | 135.9511 us | 5,751.09 us | 6,327.91 us | 6,033.07 us |    6 | 101.5625 | 322.96 KB |
 MeasureRoutineRepositoryExceptionLogList | Default |    Core | .NET Core 2.0 | 8,604.88 us | 161.8934 us | 151.4352 us | 8,345.92 us | 8,868.58 us | 8,633.78 us |   10 | 140.6250 | 441.02 KB |
     MeasureRoutineRepositoryErrorLogList | Default |    Core | .NET Core 2.0 | 7,337.51 us | 107.4821 us |  89.7524 us | 7,163.47 us | 7,498.83 us | 7,324.66 us |    8 | 132.8125 |  430.8 KB |
                    MeasureRoutineLogList |     Clr |     Clr |       Default | 5,215.46 us | 123.6356 us | 132.2887 us | 5,080.58 us | 5,558.90 us | 5,167.43 us |    3 |  93.7500 |  291.7 KB |
     MeasureRoutineNoAuthorizationLogList |     Clr |     Clr |       Default |    26.94 us |   0.1790 us |   0.1674 us |    26.65 us |    27.24 us |    26.91 us |    2 |   3.3264 |  10.24 KB |
          MeasureRoutineRepositoryLogList |     Clr |     Clr |       Default | 5,756.25 us |  94.2078 us |  88.1220 us | 5,638.09 us | 5,911.30 us | 5,736.60 us |    5 | 101.5625 | 329.09 KB |
 MeasureRoutineRepositoryExceptionLogList |     Clr |     Clr |       Default | 7,165.72 us | 143.4778 us | 413.9663 us | 6,478.43 us | 8,369.00 us | 7,116.72 us |    7 | 125.0000 | 399.22 KB |
     MeasureRoutineRepositoryErrorLogList |     Clr |     Clr |       Default | 7,886.36 us | 149.9685 us | 189.6621 us | 7,567.26 us | 8,266.28 us | 7,838.98 us |    9 | 164.0625 | 522.15 KB |
