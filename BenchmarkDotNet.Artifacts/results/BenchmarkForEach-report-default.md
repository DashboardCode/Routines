
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233542 Hz, Resolution=309.2584 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  Core   : .NET Core 4.6.25211.01, 64bit RyuJIT


    Method |  Job | Runtime |       Mean |      Error |     StdDev |        Min |        Max |     Median | Rank | Allocated |
---------- |----- |-------- |-----------:|-----------:|-----------:|-----------:|-----------:|-----------:|-----:|----------:|
  TestList |  Clr |     Clr | 5,168.4 ns | 106.799 ns | 104.891 ns | 5,048.1 ns | 5,441.1 ns | 5,139.6 ns |    3 |       0 B |
 TestArray |  Clr |     Clr |   704.7 ns |   6.389 ns |   5.977 ns |   694.7 ns |   713.1 ns |   704.7 ns |    1 |       0 B |
  TestList | Core |    Core | 5,249.7 ns |  83.718 ns |  78.310 ns | 5,106.1 ns | 5,359.1 ns | 5,264.5 ns |    4 |       0 B |
 TestArray | Core |    Core |   714.9 ns |  13.612 ns |  13.368 ns |   686.4 ns |   737.6 ns |   717.8 ns |    2 |       0 B |
