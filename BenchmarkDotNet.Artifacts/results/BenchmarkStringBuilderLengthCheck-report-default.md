
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


          Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank | Allocated |
---------------- |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|----------:|
   CheckWithBool |  Clr |     Clr | 4.597 ns | 0.0662 ns | 0.0620 ns | 4.532 ns | 4.714 ns | 4.574 ns |    1 |       0 B |
 CheckWithLength |  Clr |     Clr | 5.178 ns | 0.0496 ns | 0.0464 ns | 5.092 ns | 5.250 ns | 5.165 ns |    3 |       0 B |
   CheckWithBool | Core |    Core | 4.540 ns | 0.0938 ns | 0.0877 ns | 4.410 ns | 4.672 ns | 4.547 ns |    1 |       0 B |
 CheckWithLength | Core |    Core | 4.853 ns | 0.0832 ns | 0.0778 ns | 4.761 ns | 4.972 ns | 4.827 ns |    2 |       0 B |
