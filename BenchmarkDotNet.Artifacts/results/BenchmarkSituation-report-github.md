``` ini

BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |                          Method |  Job | Runtime |       Mean |      Error |     StdDev |        Min |        Max |     Median | Rank |    Gen 0 | Allocated |
 |-------------------------------- |----- |-------- |-----------:|-----------:|-----------:|-----------:|-----------:|-----------:|-----:|---------:|----------:|
 |  B8000SerializeBytesToJsonArray |  Clr |     Clr | 709.285 us |  7.7037 us |  6.8291 us | 700.721 us | 727.546 us | 708.230 us |    3 | 110.2865 | 369.58 kB |
 |  B0064SerializeBytesToJsonArray |  Clr |     Clr |   6.270 us |  0.1167 us |  0.1637 us |   5.987 us |   6.593 us |   6.284 us |    2 |   0.9939 |   3.34 kB |
 | B8000SerializeBytesToJsonArray2 |  Clr |     Clr | 725.306 us |  4.6631 us |  3.8939 us | 721.126 us | 735.917 us | 724.699 us |    4 | 110.2865 | 369.69 kB |
 | B0064SerializeBytesToJsonArray2 |  Clr |     Clr |   6.228 us |  0.1268 us |  0.1778 us |   6.055 us |   6.688 us |   6.157 us |    2 |   0.9842 |   3.33 kB |
 |  B8000SerializeBytesToJsonArray | Core |    Core | 705.624 us |  5.5324 us |  4.6198 us | 697.881 us | 711.692 us | 705.501 us |    3 | 110.0260 | 368.86 kB |
 |  B0064SerializeBytesToJsonArray | Core |    Core |   6.246 us |  0.1231 us |  0.1685 us |   6.027 us |   6.603 us |   6.226 us |    2 |   0.9923 |    3.3 kB |
 | B8000SerializeBytesToJsonArray2 | Core |    Core | 713.077 us | 11.6304 us | 10.8791 us | 701.225 us | 735.819 us | 709.307 us |    3 | 110.0260 | 368.83 kB |
 | B0064SerializeBytesToJsonArray2 | Core |    Core |   6.057 us |  0.1202 us |  0.1336 us |   5.788 us |   6.207 us |   6.092 us |    1 |   0.9959 |   3.33 kB |
