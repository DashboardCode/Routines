``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.192)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233537 Hz, Resolution=309.2589 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host] : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2600.0
  Core   : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|         Method |  Job | Runtime |         Mean |       Error |      StdDev |          Min |         Max |       Median | Rank |  Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|--------------- |----- |-------- |-------------:|------------:|------------:|-------------:|------------:|-------------:|-----:|-------:|-------:|-------:|----------:|
| CompletedAwait |  Clr |     Clr |    96.866 ns |   0.5636 ns |   0.4996 ns |    96.170 ns |    97.95 ns |    96.828 ns |    7 | 0.0075 |      - |      - |      24 B |
|      Completed |  Clr |     Clr |    13.151 ns |   0.1578 ns |   0.1399 ns |    12.912 ns |    13.42 ns |    13.140 ns |    2 | 0.0076 |      - |      - |      24 B |
|         Pragma |  Clr |     Clr |    90.477 ns |   0.4267 ns |   0.3991 ns |    89.891 ns |    91.39 ns |    90.448 ns |    6 | 0.0075 |      - |      - |      24 B |
|     FromResult |  Clr |     Clr |   115.587 ns |   1.6394 ns |   1.5335 ns |   113.595 ns |   118.42 ns |   115.469 ns |    8 | 0.0584 |      - |      - |     184 B |
|          Yield |  Clr |     Clr | 1,864.915 ns |  36.2738 ns |  38.8126 ns | 1,799.799 ns | 1,937.75 ns | 1,858.878 ns |    9 | 0.0877 | 0.0038 | 0.0019 |     320 B |
| CompletedAwait | Core |    Core |    37.822 ns |   0.3416 ns |   0.3195 ns |    37.341 ns |    38.39 ns |    37.811 ns |    3 | 0.0076 |      - |      - |      24 B |
|      Completed | Core |    Core |     9.965 ns |   0.1787 ns |   0.1584 ns |     9.741 ns |    10.35 ns |     9.942 ns |    1 | 0.0076 |      - |      - |      24 B |
|         Pragma | Core |    Core |    46.747 ns |   0.9722 ns |   2.8360 ns |    40.697 ns |    54.07 ns |    46.743 ns |    4 | 0.0076 |      - |      - |      24 B |
|     FromResult | Core |    Core |    52.795 ns |   0.5847 ns |   0.5183 ns |    52.139 ns |    53.86 ns |    52.734 ns |    5 | 0.0533 |      - |      - |     168 B |
|          Yield | Core |    Core | 2,826.584 ns | 117.3361 ns | 336.6593 ns | 2,257.315 ns | 3,710.08 ns | 2,776.808 ns |   10 | 0.0916 |      - |      - |     296 B |
