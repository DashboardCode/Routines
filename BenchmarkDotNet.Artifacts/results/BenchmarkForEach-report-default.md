
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 1 (10.0.14393)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233538 Hz, Resolution=309.2588 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25211.01, 64bit RyuJIT


    Method |  Job | Runtime |       Mean |      Error |    StdDev |        Min |        Max |     Median | Rank | Allocated |
---------- |----- |-------- |-----------:|-----------:|----------:|-----------:|-----------:|-----------:|-----:|----------:|
  TestList |  Clr |     Clr | 5,153.3 ns |  34.002 ns | 31.806 ns | 5,119.2 ns | 5,213.4 ns | 5,135.9 ns |    3 |       0 B |
 TestArray |  Clr |     Clr |   730.1 ns |   6.962 ns |  6.512 ns |   722.4 ns |   743.9 ns |   729.5 ns |    2 |       0 B |
  TestList | Core |    Core | 5,188.4 ns | 102.816 ns | 96.174 ns | 5,070.3 ns | 5,342.6 ns | 5,185.6 ns |    3 |       0 B |
 TestArray | Core |    Core |   709.0 ns |   6.126 ns |  5.730 ns |   700.8 ns |   718.6 ns |   708.1 ns |    1 |       0 B |
