
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


             Method |  Job | Runtime |      Mean |     Error |    StdDev |       Min |       Max |    Median | Rank | Allocated |
------------------- |----- |-------- |----------:|----------:|----------:|----------:|----------:|----------:|-----:|----------:|
        F16_Foreach |  Clr |     Clr |  26.00 us | 0.2208 us | 0.2065 us |  25.80 us |  26.41 us |  25.90 us |    3 |       0 B |
            F16_For |  Clr |     Clr |  25.85 us | 0.0881 us | 0.0781 us |  25.74 us |  26.02 us |  25.85 us |    2 |       0 B |
     F16_Enumerator |  Clr |     Clr | 228.29 us | 1.1703 us | 1.0375 us | 227.15 us | 230.81 us | 227.99 us |    7 |      36 B |
 F16_ListEnumerator |  Clr |     Clr |  61.46 us | 0.5350 us | 0.4743 us |  60.98 us |  62.73 us |  61.28 us |    6 |       1 B |
        F16_Foreach | Core |    Core |  23.65 us | 0.1203 us | 0.1125 us |  23.47 us |  23.77 us |  23.68 us |    1 |       0 B |
            F16_For | Core |    Core |  28.35 us | 0.2143 us | 0.1900 us |  28.13 us |  28.82 us |  28.32 us |    4 |       0 B |
     F16_Enumerator | Core |    Core | 237.46 us | 2.5835 us | 2.4166 us | 233.53 us | 241.73 us | 236.76 us |    8 |      36 B |
 F16_ListEnumerator | Core |    Core |  60.55 us | 0.3513 us | 0.2934 us |  60.27 us |  61.36 us |  60.47 us |    5 |       0 B |
