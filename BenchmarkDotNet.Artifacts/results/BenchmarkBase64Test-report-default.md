
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


      Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |   Gen 0 | Allocated |
------------ |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|--------:|----------:|
  Custom8000 |  Clr |     Clr | 65.19 us | 0.3342 us | 0.3126 us | 64.43 us | 65.73 us | 65.22 us |    3 | 46.2484 | 146.09 kB |
 Convert8000 |  Clr |     Clr | 21.40 us | 0.2492 us | 0.2331 us | 21.07 us | 21.80 us | 21.36 us |    2 | 25.1343 |  78.59 kB |
  Custom8000 | Core |    Core | 69.82 us | 0.6224 us | 0.5822 us | 68.91 us | 70.75 us | 69.88 us |    4 | 46.1833 | 145.78 kB |
 Convert8000 | Core |    Core | 20.05 us | 0.3035 us | 0.2370 us | 19.83 us | 20.71 us | 19.98 us |    1 | 25.0936 |  78.47 kB |
