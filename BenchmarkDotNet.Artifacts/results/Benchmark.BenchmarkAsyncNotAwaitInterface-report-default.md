
BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.192)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233537 Hz, Resolution=309.2589 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2600.0
  Core   : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


         Method |  Job | Runtime |         Mean |       Error |      StdDev |       Median |          Min |          Max | Rank |  Gen 0 |  Gen 1 |  Gen 2 | Allocated |
--------------- |----- |-------- |-------------:|------------:|------------:|-------------:|-------------:|-------------:|-----:|-------:|-------:|-------:|----------:|
 CompletedAwait |  Clr |     Clr |    95.253 ns |   0.7491 ns |   0.6641 ns |    95.100 ns |    94.461 ns |    96.557 ns |    7 | 0.0075 |      - |      - |      24 B |
      Completed |  Clr |     Clr |    12.036 ns |   0.0659 ns |   0.0617 ns |    12.026 ns |    11.931 ns |    12.154 ns |    2 | 0.0076 |      - |      - |      24 B |
         Pragma |  Clr |     Clr |    87.868 ns |   0.3923 ns |   0.3670 ns |    87.789 ns |    87.336 ns |    88.683 ns |    6 | 0.0075 |      - |      - |      24 B |
     FromResult |  Clr |     Clr |   107.009 ns |   0.6671 ns |   0.6240 ns |   107.009 ns |   106.204 ns |   108.247 ns |    8 | 0.0584 |      - |      - |     184 B |
          Yield |  Clr |     Clr | 1,766.843 ns |  26.5216 ns |  24.8083 ns | 1,770.383 ns | 1,705.386 ns | 1,800.653 ns |    9 | 0.0877 | 0.0038 | 0.0019 |     320 B |
 CompletedAwait | Core |    Core |    37.201 ns |   0.1961 ns |   0.1739 ns |    37.227 ns |    36.970 ns |    37.559 ns |    4 | 0.0076 |      - |      - |      24 B |
      Completed | Core |    Core |     9.017 ns |   0.0690 ns |   0.0577 ns |     9.010 ns |     8.925 ns |     9.128 ns |    1 | 0.0076 |      - |      - |      24 B |
         Pragma | Core |    Core |    34.118 ns |   0.4576 ns |   0.4281 ns |    34.259 ns |    33.437 ns |    34.792 ns |    3 | 0.0076 |      - |      - |      24 B |
     FromResult | Core |    Core |    46.953 ns |   1.2728 ns |   1.1905 ns |    46.467 ns |    45.674 ns |    49.868 ns |    5 | 0.0533 |      - |      - |     168 B |
          Yield | Core |    Core | 2,480.980 ns | 199.4416 ns | 575.4347 ns | 2,291.978 ns | 1,810.644 ns | 4,085.196 ns |   10 | 0.0916 |      - |      - |     296 B |
