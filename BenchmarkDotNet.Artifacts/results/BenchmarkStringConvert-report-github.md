``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 1 (10.0.14393)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                        Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |  Gen 0 | Allocated |
 |------------------------------ |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|-------:|----------:|
 |                 StringDecimal |  Clr |     Clr | 235.4 ns | 3.3599 ns | 2.9784 ns | 229.3 ns | 241.5 ns | 235.7 ns |    5 | 0.0277 |      88 B |
 |                   StringFloat |  Clr |     Clr | 389.3 ns | 3.2346 ns | 2.8674 ns | 386.0 ns | 394.8 ns | 387.9 ns |    7 | 0.0148 |      48 B |
 |          StringDecimalConvert |  Clr |     Clr | 237.0 ns | 2.9265 ns | 2.7374 ns | 234.4 ns | 242.1 ns | 236.3 ns |    5 | 0.0277 |      88 B |
 |            StringFloatConvert |  Clr |     Clr | 387.1 ns | 5.8869 ns | 5.5066 ns | 380.7 ns | 399.6 ns | 386.3 ns |    7 | 0.0148 |      48 B |
 | StringDecimalConvertInvariant |  Clr |     Clr | 216.6 ns | 1.9493 ns | 1.7280 ns | 213.9 ns | 220.6 ns | 216.2 ns |    2 | 0.0279 |      88 B |
 |   StringFloatConvertInvariant |  Clr |     Clr | 370.3 ns | 4.7017 ns | 4.3980 ns | 365.6 ns | 377.8 ns | 368.4 ns |    6 | 0.0148 |      48 B |
 |                 StringDecimal | Core |    Core | 233.5 ns | 0.8542 ns | 0.7990 ns | 231.7 ns | 234.9 ns | 233.5 ns |    4 | 0.0279 |      88 B |
 |                   StringFloat | Core |    Core | 455.0 ns | 4.5542 ns | 4.2600 ns | 449.6 ns | 464.3 ns | 455.2 ns |   10 | 0.0148 |      48 B |
 |          StringDecimalConvert | Core |    Core | 228.6 ns | 0.8987 ns | 0.7017 ns | 227.4 ns | 229.8 ns | 228.6 ns |    3 | 0.0279 |      88 B |
 |            StringFloatConvert | Core |    Core | 443.0 ns | 1.7280 ns | 1.6164 ns | 440.6 ns | 446.9 ns | 442.9 ns |    9 | 0.0148 |      48 B |
 | StringDecimalConvertInvariant | Core |    Core | 208.8 ns | 0.9151 ns | 0.8112 ns | 207.9 ns | 210.3 ns | 208.6 ns |    1 | 0.0279 |      88 B |
 |   StringFloatConvertInvariant | Core |    Core | 417.7 ns | 1.2913 ns | 1.1447 ns | 415.2 ns | 419.7 ns | 417.6 ns |    8 | 0.0148 |      48 B |
