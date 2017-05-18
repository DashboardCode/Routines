
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


              Method |  Job | Runtime |     Mean |    Error |   StdDev |      Min |      Max |   Median | Rank |  Gen 0 | Allocated |
-------------------- |----- |-------- |---------:|---------:|---------:|---------:|---------:|---------:|-----:|-------:|----------:|
 Dictionary_10from20 |  Clr |     Clr | 783.9 ns | 14.00 ns | 13.09 ns | 755.9 ns | 807.6 ns | 783.6 ns |    1 | 0.3030 |     992 B |
       List_10from20 |  Clr |     Clr | 941.9 ns | 11.18 ns | 10.46 ns | 923.0 ns | 962.0 ns | 944.8 ns |    3 | 0.6545 |    2096 B |
 Dictionary_10from20 | Core |    Core | 794.2 ns | 16.98 ns | 23.80 ns | 761.2 ns | 852.5 ns | 794.0 ns |    1 | 0.3030 |     989 B |
       List_10from20 | Core |    Core | 923.4 ns | 19.62 ns | 28.76 ns | 883.5 ns | 997.0 ns | 920.8 ns |    2 | 0.6535 |    2090 B |
