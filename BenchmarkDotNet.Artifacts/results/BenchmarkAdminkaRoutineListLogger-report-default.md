
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


                                   Method |     Job | Runtime |     Toolchain |        Mean |       Error |      StdDev |      Median |         Min |         Max | Rank |    Gen 0 | Allocated |
----------------------------------------- |-------- |-------- |-------------- |------------:|------------:|------------:|------------:|------------:|------------:|-----:|---------:|----------:|
                    MeasureRoutineLogList | Default |    Core | .NET Core 2.0 | 5,211.88 us | 134.5184 us | 377.2040 us | 5,214.65 us | 4,639.13 us | 6,376.98 us |    4 |  85.9375 | 287.59 KB |
     MeasureRoutineNoAuthorizationLogList | Default |    Core | .NET Core 2.0 |    13.71 us |   0.2588 us |   0.2542 us |    13.70 us |    13.39 us |    14.36 us |    1 |   2.2583 |   6.94 KB |
          MeasureRoutineRepositoryLogList | Default |    Core | .NET Core 2.0 | 5,749.37 us | 155.5720 us | 443.8556 us | 5,641.79 us | 5,166.67 us | 6,907.78 us |    5 | 101.5625 | 323.23 KB |
 MeasureRoutineRepositoryExceptionLogList | Default |    Core | .NET Core 2.0 | 8,243.15 us | 169.1563 us | 427.4796 us | 8,076.62 us | 7,730.21 us | 9,576.97 us |    7 | 140.6250 | 440.65 KB |
                    MeasureRoutineLogList |     Clr |     Clr |       Default | 4,761.33 us |  93.3273 us | 111.0995 us | 4,738.47 us | 4,557.32 us | 5,003.88 us |    3 |  93.7500 | 288.88 KB |
     MeasureRoutineNoAuthorizationLogList |     Clr |     Clr |       Default |    21.97 us |   0.1534 us |   0.1360 us |    21.94 us |    21.77 us |    22.27 us |    2 |   2.8687 |    8.9 KB |
          MeasureRoutineRepositoryLogList |     Clr |     Clr |       Default | 5,267.90 us | 100.9269 us | 116.2276 us | 5,264.39 us | 4,997.40 us | 5,501.40 us |    4 |  93.7500 | 327.01 KB |
 MeasureRoutineRepositoryExceptionLogList |     Clr |     Clr |       Default | 6,202.20 us | 123.9406 us | 169.6513 us | 6,164.96 us | 5,947.54 us | 6,549.13 us |    6 | 125.0000 | 396.54 KB |
