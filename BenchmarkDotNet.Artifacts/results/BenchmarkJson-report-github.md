``` ini

BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |  Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median |  Gen 0 | Allocated |
 |-------- |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-------:|----------:|
 | Routine |  Clr |     Clr | 31.46 us | 0.2989 us | 0.2796 us | 31.10 us | 32.04 us | 31.43 us | 2.9907 |  11.13 kB |
 | JsonNet |  Clr |     Clr | 47.00 us | 0.8410 us | 0.7456 us | 46.03 us | 48.61 us | 46.87 us | 2.9175 |   11.2 kB |
 | Routine | Core |    Core | 35.46 us | 0.6901 us | 0.8974 us | 33.98 us | 37.03 us | 35.60 us | 2.7188 |   9.74 kB |
 | JsonNet | Core |    Core | 49.26 us | 0.8823 us | 0.8253 us | 47.94 us | 50.72 us | 49.20 us | 2.6274 |   9.81 kB |
