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
 |     CheckObject |  Clr |     Clr | 80.1984 ns | 0.9091 ns | 0.8504 ns | 79.1017 ns | 82.0118 ns | 79.8825 ns |    4 | 0.0058 |      24 B |
 |   CheckNullable |  Clr |     Clr |  0.2982 ns | 0.0354 ns | 0.0331 ns |  0.2543 ns |  0.3570 ns |  0.2920 ns |    2 |      - |       0 B |
 | CheckNullableEq |  Clr |     Clr |  0.2791 ns | 0.0062 ns | 0.0058 ns |  0.2693 ns |  0.2870 ns |  0.2793 ns |    1 |      - |       0 B |
 |     CheckObject | Core |    Core | 80.1019 ns | 0.2832 ns | 0.2649 ns | 79.5440 ns | 80.5013 ns | 80.1312 ns |    4 | 0.0060 |      24 B |
 |   CheckNullable | Core |    Core |  2.0300 ns | 0.0166 ns | 0.0156 ns |  2.0093 ns |  2.0521 ns |  2.0268 ns |    3 |      - |       0 B |
 | CheckNullableEq | Core |    Core |  2.0338 ns | 0.0490 ns | 0.0434 ns |  1.9945 ns |  2.1395 ns |  2.0123 ns |    3 |      - |       0 B |
