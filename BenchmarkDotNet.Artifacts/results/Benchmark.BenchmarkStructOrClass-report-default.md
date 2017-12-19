
BenchmarkDotNet=v0.10.10, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
.NET Core SDK=2.0.3
  [Host] : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2115.0
  Core   : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


            Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |  Gen 0 | Allocated |
------------------ |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|-------:|----------:|
  TestStructReturn |  Clr |     Clr | 17.57 ns | 0.1960 ns | 0.1834 ns | 17.25 ns | 17.89 ns | 17.55 ns |    4 | 0.0127 |      40 B |
   TestClassReturn |  Clr |     Clr | 21.93 ns | 0.4554 ns | 0.5244 ns | 21.17 ns | 23.26 ns | 21.86 ns |    5 | 0.0229 |      72 B |
 TestStructReturn8 |  Clr |     Clr | 38.99 ns | 0.8302 ns | 1.4097 ns | 37.36 ns | 42.35 ns | 38.50 ns |    8 | 0.0127 |      40 B |
  TestClassReturn8 |  Clr |     Clr | 23.69 ns | 0.5373 ns | 0.6987 ns | 22.70 ns | 25.24 ns | 23.37 ns |    6 | 0.0305 |      96 B |
  TestStructReturn | Core |    Core | 12.28 ns | 0.1882 ns | 0.1760 ns | 11.92 ns | 12.57 ns | 12.30 ns |    1 | 0.0127 |      40 B |
   TestClassReturn | Core |    Core | 15.33 ns | 0.4343 ns | 0.4063 ns | 14.83 ns | 16.44 ns | 15.31 ns |    2 | 0.0229 |      72 B |
 TestStructReturn8 | Core |    Core | 34.11 ns | 0.7089 ns | 1.4954 ns | 31.52 ns | 36.81 ns | 34.03 ns |    7 | 0.0127 |      40 B |
  TestClassReturn8 | Core |    Core | 17.04 ns | 0.2299 ns | 0.2150 ns | 16.68 ns | 17.41 ns | 16.98 ns |    3 | 0.0305 |      96 B |
