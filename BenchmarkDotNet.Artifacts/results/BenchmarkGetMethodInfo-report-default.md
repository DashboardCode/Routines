
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 1 (10.0.14393)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25211.01, 64bit RyuJIT


          Method |  Job | Runtime |     Mean |     Error |    StdDev |   Median |      Min |      Max | Rank |  Gen 0 | Allocated |
---------------- |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|-------:|----------:|
 DelegateBuilded |  Clr |     Clr | 59.26 ns | 0.2291 ns | 0.1789 ns | 59.25 ns | 59.00 ns | 59.69 ns |    3 | 0.1500 |     472 B |
      Expression |  Clr |     Clr | 59.72 ns | 1.1989 ns | 1.2828 ns | 59.50 ns | 57.55 ns | 62.72 ns |    3 | 0.1500 |     472 B |
 DelegateBuilded | Core |    Core | 52.56 ns | 1.0877 ns | 1.5943 ns | 52.36 ns | 50.30 ns | 55.72 ns |    1 | 0.1500 |     472 B |
      Expression | Core |    Core | 55.40 ns | 1.4686 ns | 4.2373 ns | 53.98 ns | 50.78 ns | 66.79 ns |    2 | 0.1500 |     472 B |
