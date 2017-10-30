``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
.NET Core SDK=2.0.2
  [Host]     : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  Job-EOKNZQ : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  Job-RAFWZX : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2115.0


```
 |      Method |   Toolchain |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |   Gen 0 | Allocated |
 |------------ |------------ |---------:|----------:|----------:|---------:|---------:|---------:|-----:|--------:|----------:|
 |  FilterLinq |  CoreCsProj | 123.3 us | 1.4565 us | 1.3624 us | 121.2 us | 125.9 us | 123.0 us |    1 | 20.7520 |   64.4 KB |
 | FilterPLinq |  CoreCsProj | 435.2 us | 6.3337 us | 5.9246 us | 428.8 us | 448.8 us | 433.8 us |    4 | 44.9219 |  90.25 KB |
 |  FilterLinq | CsProjnet47 | 142.1 us | 0.9836 us | 0.8719 us | 140.9 us | 143.5 us | 142.3 us |    2 | 20.7520 |  64.47 KB |
 | FilterPLinq | CsProjnet47 | 398.1 us | 5.6262 us | 4.9875 us | 391.7 us | 408.3 us | 396.8 us |    3 | 45.2091 | 139.97 KB |
