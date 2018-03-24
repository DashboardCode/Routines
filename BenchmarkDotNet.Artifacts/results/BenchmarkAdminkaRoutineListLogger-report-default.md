
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host] : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0

Job=Clr  Runtime=Clr  

                                   Method |        Mean |       Error |      StdDev |         Min |         Max |      Median | Rank |    Gen 0 | Allocated |
----------------------------------------- |------------:|------------:|------------:|------------:|------------:|------------:|-----:|---------:|----------:|
                    MeasureRoutineLogList | 5,772.98 us | 139.1975 us | 116.2362 us | 5,625.90 us | 6,085.68 us | 5,737.06 us |    2 | 164.0625 | 527.62 KB |
     MeasureRoutineNoAuthorizationLogList |    39.23 us |   0.7598 us |   0.8130 us |    38.19 us |    40.90 us |    39.03 us |    1 |   5.5542 |  17.19 KB |
          MeasureRoutineRepositoryLogList | 6,384.86 us | 125.3799 us | 272.5656 us | 5,827.29 us | 7,002.67 us | 6,389.52 us |    3 | 187.5000 | 598.62 KB |
 MeasureRoutineRepositoryExceptionLogList | 7,177.40 us | 122.0751 us | 108.2164 us | 7,008.69 us | 7,407.82 us | 7,177.67 us |    4 | 195.3125 | 618.71 KB |
