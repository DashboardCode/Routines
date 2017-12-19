``` ini

BenchmarkDotNet=v0.10.10, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
.NET Core SDK=2.0.3
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2115.0
  Core   : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|         Method |  Job | Runtime |         Mean |      Error |      StdDev |          Min |          Max |       Median | Rank |  Gen 0 |  Gen 1 | Allocated |
|--------------- |----- |-------- |-------------:|-----------:|------------:|-------------:|-------------:|-------------:|-----:|-------:|-------:|----------:|
| CompletedAwait |  Clr |     Clr |    97.775 ns |  1.9912 ns |   2.6582 ns |    93.746 ns |   104.485 ns |    98.416 ns |    6 | 0.0075 |      - |      24 B |
|      Completed |  Clr |     Clr |    12.254 ns |  0.2852 ns |   0.7904 ns |    11.005 ns |    14.461 ns |    12.158 ns |    2 | 0.0076 |      - |      24 B |
|         Pragma |  Clr |     Clr |    91.666 ns |  1.8630 ns |   2.2879 ns |    87.121 ns |    96.547 ns |    91.648 ns |    5 | 0.0075 |      - |      24 B |
|          Yield |  Clr |     Clr | 1,913.335 ns | 38.2067 ns |  53.5605 ns | 1,843.719 ns | 2,045.178 ns | 1,909.520 ns |    7 | 0.0877 | 0.0038 |     320 B |
| CompletedAwait | Core |    Core |    37.145 ns |  0.5648 ns |   0.5283 ns |    36.400 ns |    37.992 ns |    37.187 ns |    4 | 0.0076 |      - |      24 B |
|      Completed | Core |    Core |     8.699 ns |  0.1704 ns |   0.1511 ns |     8.395 ns |     8.955 ns |     8.701 ns |    1 | 0.0076 |      - |      24 B |
|         Pragma | Core |    Core |    35.132 ns |  0.7197 ns |   0.7391 ns |    34.033 ns |    36.503 ns |    35.061 ns |    3 | 0.0076 |      - |      24 B |
|          Yield | Core |    Core | 2,128.040 ns | 64.3668 ns | 184.6804 ns | 1,870.026 ns | 2,651.805 ns | 2,107.816 ns |    8 | 0.0916 |      - |     296 B |
