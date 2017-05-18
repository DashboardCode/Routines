``` ini

BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |            Method |  Job | Runtime |       Mean |     Error |    StdDev |        Min |        Max |     Median | Rank |    Gen 0 | Allocated |
 |------------------ |----- |-------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-----:|---------:|----------:|
 | StringBuilder8000 |  Clr |     Clr | 728.634 us | 9.2615 us | 8.6632 us | 715.071 us | 740.372 us | 727.286 us |    5 | 110.2865 | 369.53 kB |
 |   StringBuilder64 |  Clr |     Clr |   5.960 us | 0.0736 us | 0.0689 us |   5.868 us |   6.090 us |   5.972 us |    1 |   0.9923 |   3.33 kB |
 |    CharBuffer8000 |  Clr |     Clr | 788.176 us | 3.5420 us | 3.3132 us | 781.797 us | 793.403 us | 787.568 us |    7 | 128.8411 |  424.4 kB |
 |      CharBuffer64 |  Clr |     Clr |   6.483 us | 0.0563 us | 0.0526 us |   6.428 us |   6.587 us |   6.469 us |    3 |   1.0681 |   3.59 kB |
 | StringBuilder8000 | Core |    Core | 686.731 us | 2.5280 us | 2.2410 us | 681.800 us | 690.091 us | 687.160 us |    4 | 109.7656 | 368.86 kB |
 |   StringBuilder64 | Core |    Core |   5.989 us | 0.0485 us | 0.0430 us |   5.921 us |   6.066 us |   5.989 us |    1 |   0.9842 |   3.32 kB |
 |    CharBuffer8000 | Core |    Core | 748.585 us | 9.1814 us | 8.1391 us | 737.032 us | 766.557 us | 749.684 us |    6 | 128.5807 | 423.56 kB |
 |      CharBuffer64 | Core |    Core |   6.117 us | 0.1120 us | 0.1048 us |   5.931 us |   6.372 us |   6.107 us |    2 |   1.1004 |   3.58 kB |
