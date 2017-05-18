
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1637.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


                Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |  Gen 0 | Allocated |
---------------------- |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|-------:|----------:|
            StringJoin |  Clr |     Clr | 29.48 us | 0.5675 us | 0.6970 us | 28.77 us | 30.91 us | 29.13 us |    7 | 4.9133 |   16.3 kB |
 SeparatorSubstitution |  Clr |     Clr | 18.58 us | 0.3714 us | 0.6791 us | 17.57 us | 20.41 us | 18.48 us |    5 | 5.1154 |  16.27 kB |
     SeparatorStepBack |  Clr |     Clr | 17.75 us | 0.3440 us | 0.5043 us | 17.08 us | 18.96 us | 17.78 us |    3 | 4.9780 |  16.27 kB |
    SeparatorStepBack2 |  Clr |     Clr | 17.37 us | 0.2143 us | 0.2005 us | 17.04 us | 17.70 us | 17.37 us |    2 | 4.9540 |  16.27 kB |
            Enumerable |  Clr |     Clr | 18.50 us | 0.3664 us | 0.5015 us | 17.78 us | 19.47 us | 18.46 us |    5 | 4.9995 |  16.27 kB |
            StringJoin | Core |    Core | 27.37 us | 0.3246 us | 0.3037 us | 26.70 us | 27.84 us | 27.32 us |    6 | 4.9540 |  16.26 kB |
 SeparatorSubstitution | Core |    Core | 17.50 us | 0.2659 us | 0.2487 us | 17.15 us | 17.96 us | 17.46 us |    2 | 4.9296 |  16.22 kB |
     SeparatorStepBack | Core |    Core | 16.26 us | 0.1946 us | 0.1820 us | 15.94 us | 16.50 us | 16.29 us |    1 | 4.9851 |  16.22 kB |
    SeparatorStepBack2 | Core |    Core | 16.14 us | 0.3154 us | 0.4102 us | 15.65 us | 17.02 us | 16.05 us |    1 | 5.1823 |  16.22 kB |
            Enumerable | Core |    Core | 18.16 us | 0.3043 us | 0.2846 us | 17.78 us | 18.67 us | 18.03 us |    4 | 4.9377 |  16.22 kB |
