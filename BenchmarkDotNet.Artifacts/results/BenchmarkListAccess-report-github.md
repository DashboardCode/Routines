``` ini

BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |          Method |  Job | Runtime |         Mean |       Error |      StdDev |          Min |          Max |       Median | Rank |  Gen 0 | Allocated |
 |---------------- |----- |-------- |-------------:|------------:|------------:|-------------:|-------------:|-------------:|-----:|-------:|----------:|
 |    Dictionary10 |  Clr |     Clr |     25.68 ns |   0.5036 ns |   0.4465 ns |     25.10 ns |     26.67 ns |     25.63 ns |    1 |      - |       0 B |
 |   Dictionary100 |  Clr |     Clr |     25.41 ns |   0.2891 ns |   0.2705 ns |     25.08 ns |     25.94 ns |     25.34 ns |    1 |      - |       0 B |
 |  Dictionary1000 |  Clr |     Clr |     26.24 ns |   0.3007 ns |   0.2813 ns |     25.82 ns |     26.82 ns |     26.23 ns |    2 |      - |       0 B |
 | Dictionary10000 |  Clr |     Clr |     27.68 ns |   0.5035 ns |   0.4710 ns |     27.02 ns |     28.60 ns |     27.84 ns |    5 |      - |       0 B |
 |          List10 |  Clr |     Clr |     52.29 ns |   1.1142 ns |   2.2252 ns |     49.36 ns |     57.83 ns |     51.99 ns |    7 | 0.0271 |      88 B |
 |         List100 |  Clr |     Clr |    205.05 ns |   3.5276 ns |   3.2997 ns |    202.07 ns |    213.23 ns |    203.69 ns |    9 | 0.0245 |      88 B |
 |        List1000 |  Clr |     Clr |  1,770.84 ns |  23.4166 ns |  21.9039 ns |  1,747.73 ns |  1,818.31 ns |  1,760.28 ns |   11 | 0.0028 |      88 B |
 |       List10000 |  Clr |     Clr | 17,391.58 ns | 199.7064 ns | 186.8054 ns | 17,185.68 ns | 17,747.07 ns | 17,312.10 ns |   13 |      - |      88 B |
 |    Dictionary10 | Core |    Core |     25.85 ns |   0.2299 ns |   0.2038 ns |     25.60 ns |     26.29 ns |     25.80 ns |    1 |      - |       0 B |
 |   Dictionary100 | Core |    Core |     26.66 ns |   0.4289 ns |   0.4012 ns |     25.67 ns |     27.12 ns |     26.74 ns |    3 |      - |       0 B |
 |  Dictionary1000 | Core |    Core |     25.37 ns |   0.1622 ns |   0.1438 ns |     25.19 ns |     25.60 ns |     25.29 ns |    1 |      - |       0 B |
 | Dictionary10000 | Core |    Core |     27.32 ns |   0.4949 ns |   0.4629 ns |     26.64 ns |     28.17 ns |     27.26 ns |    4 |      - |       0 B |
 |          List10 | Core |    Core |     47.67 ns |   0.4176 ns |   0.3906 ns |     46.96 ns |     48.29 ns |     47.56 ns |    6 | 0.0271 |      88 B |
 |         List100 | Core |    Core |    193.87 ns |   3.8302 ns |   4.8440 ns |    187.86 ns |    206.24 ns |    192.06 ns |    8 | 0.0244 |      88 B |
 |        List1000 | Core |    Core |  1,684.57 ns |  20.6439 ns |  19.3103 ns |  1,649.19 ns |  1,709.42 ns |  1,688.06 ns |   10 | 0.0043 |      88 B |
 |       List10000 | Core |    Core | 16,222.76 ns | 305.7644 ns | 327.1644 ns | 15,790.24 ns | 16,988.02 ns | 16,136.64 ns |   12 |      - |      88 B |
