
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Job-GGHVXY : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0


                                    Method |     Job | Runtime |     Toolchain |        Mean |       Error |      StdDev |      Median |         Min |         Max | Rank |    Gen 0 |   Gen 1 | Allocated |
------------------------------------------ |-------- |-------- |-------------- |------------:|------------:|------------:|------------:|------------:|------------:|-----:|---------:|--------:|----------:|
                        MeasureRoutineNLog | Default |    Core | .NET Core 2.0 |  6,604.9 us |   137.43 us |   321.24 us |  6,468.3 us |  6,175.1 us |  7,461.8 us |    3 | 109.3750 |       - | 355.04 KB |
         MeasureRoutineNoAuthorizationNLog | Default |    Core | .NET Core 2.0 |    522.6 us |    24.83 us |    73.20 us |    487.9 us |    439.8 us |    719.0 us |    1 |  12.6953 |       - |  40.71 KB |
              MeasureRoutineRepositoryNLog | Default |    Core | .NET Core 2.0 |  7,501.3 us |   165.74 us |   403.45 us |  7,346.5 us |  7,009.6 us |  8,642.7 us |    4 | 125.0000 |       - |  390.2 KB |
     MeasureRoutineRepositoryExceptionNLog | Default |    Core | .NET Core 2.0 | 12,332.4 us |   243.75 us |   407.25 us | 12,276.6 us | 11,621.4 us | 13,435.9 us |    5 | 187.5000 | 62.5000 | 939.45 KB |
 MeasureRoutineRepositoryExceptionMailNLog | Default |    Core | .NET Core 2.0 | 41,304.1 us |   818.61 us | 1,762.15 us | 40,919.7 us | 38,778.5 us | 46,315.1 us |    9 | 250.0000 |       - | 778.65 KB |
                        MeasureRoutineNLog |     Clr |     Clr |       Default | 14,350.0 us |   273.71 us |   417.98 us | 14,319.1 us | 13,713.3 us | 15,633.5 us |    6 | 109.3750 |       - | 356.53 KB |
         MeasureRoutineNoAuthorizationNLog |     Clr |     Clr |       Default |  4,518.4 us |    85.95 us |   105.55 us |  4,504.6 us |  4,332.2 us |  4,766.4 us |    2 |   7.8125 |       - |   42.3 KB |
              MeasureRoutineRepositoryNLog |     Clr |     Clr |       Default | 15,726.6 us |   340.56 us |   605.35 us | 15,607.6 us | 14,862.0 us | 17,456.0 us |    7 | 125.0000 |       - | 394.01 KB |
     MeasureRoutineRepositoryExceptionNLog |     Clr |     Clr |       Default | 17,368.6 us |   341.45 us |   689.74 us | 17,215.8 us | 16,472.3 us | 19,646.1 us |    8 | 250.0000 | 62.5000 | 878.01 KB |
 MeasureRoutineRepositoryExceptionMailNLog |     Clr |     Clr |       Default | 45,682.7 us | 1,021.34 us | 1,291.67 us | 45,709.5 us | 42,803.5 us | 48,991.7 us |   10 | 187.5000 |       - | 752.15 KB |
