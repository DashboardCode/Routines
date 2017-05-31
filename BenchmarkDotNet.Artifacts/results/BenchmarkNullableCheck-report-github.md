``` ini

BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |          Method |  Job | Runtime |       Mean |     Error |    StdDev |        Min |        Max |     Median | Rank |  Gen 0 | Allocated |
 |---------------- |----- |-------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-----:|-------:|----------:|
 |     CheckObject |  Clr |     Clr | 79.8397 ns | 0.6019 ns | 0.5630 ns | 79.0786 ns | 80.9902 ns | 79.6320 ns |    4 | 0.0060 |      24 B |
 |   CheckNullable |  Clr |     Clr |  0.0125 ns | 0.0043 ns | 0.0040 ns |  0.0060 ns |  0.0202 ns |  0.0122 ns |    1 |      - |       0 B |
 |    CheckGeneric |  Clr |     Clr | 81.0836 ns | 0.3934 ns | 0.3680 ns | 80.5962 ns | 81.9129 ns | 81.1074 ns |    5 | 0.0060 |      24 B |
 | CheckNullableEq |  Clr |     Clr |  0.0349 ns | 0.0217 ns | 0.0203 ns |  0.0125 ns |  0.0855 ns |  0.0253 ns |    2 |      - |       0 B |
 |     CheckObject | Core |    Core | 82.0344 ns | 0.4862 ns | 0.4548 ns | 81.2557 ns | 82.9128 ns | 82.0024 ns |    6 | 0.0059 |      24 B |
 |   CheckNullable | Core |    Core |  2.0631 ns | 0.0143 ns | 0.0134 ns |  2.0449 ns |  2.0890 ns |  2.0598 ns |    3 |      - |       0 B |
 |    CheckGeneric | Core |    Core | 82.5551 ns | 0.3273 ns | 0.2901 ns | 81.9570 ns | 82.9888 ns | 82.4965 ns |    7 | 0.0060 |      24 B |
 | CheckNullableEq | Core |    Core |  2.0611 ns | 0.0151 ns | 0.0134 ns |  2.0403 ns |  2.0879 ns |  2.0609 ns |    3 |      - |       0 B |
