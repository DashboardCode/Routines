
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25009.03, 64bit RyuJIT


               Method |  Job | Runtime |       Mean |     Error |    StdDev |        Min |        Max |     Median | Rank |   Gen 0 | Allocated |
--------------------- |----- |-------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-----:|--------:|----------:|
        B8000_Foreach |  Clr |     Clr |   4.691 us | 0.0558 us | 0.0522 us |   4.599 us |   4.761 us |   4.695 us |    1 |       - |       0 B |
            B8000_For |  Clr |     Clr |   7.033 us | 0.0685 us | 0.0641 us |   6.951 us |   7.184 us |   7.021 us |    3 |       - |       0 B |
     B8000_Enumerator |  Clr |     Clr | 673.559 us | 2.7735 us | 2.3160 us | 668.336 us | 677.526 us | 673.498 us |   13 | 50.1302 |  192090 B |
 B8000_ListEnumerator |  Clr |     Clr |  51.715 us | 0.3930 us | 0.3676 us |  51.078 us |  52.326 us |  51.788 us |    9 |       - |       0 B |
        S8000_Foreach |  Clr |     Clr |  11.093 us | 0.0784 us | 0.0734 us |  10.959 us |  11.184 us |  11.116 us |    4 |       - |       0 B |
            S8000_For |  Clr |     Clr |  13.006 us | 0.1715 us | 0.1604 us |  12.841 us |  13.331 us |  12.975 us |    7 |       - |       0 B |
     S8000_Enumerator |  Clr |     Clr | 198.873 us | 1.3635 us | 1.2754 us | 196.489 us | 201.337 us | 198.916 us |   10 |       - |      36 B |
 S8000_ListEnumerator |  Clr |     Clr |  39.787 us | 0.1494 us | 0.1248 us |  39.565 us |  40.057 us |  39.793 us |    8 |       - |       0 B |
        B8000_Foreach | Core |    Core |   4.679 us | 0.0208 us | 0.0184 us |   4.648 us |   4.715 us |   4.676 us |    1 |       - |       0 B |
            B8000_For | Core |    Core |   5.698 us | 0.0229 us | 0.0203 us |   5.668 us |   5.735 us |   5.699 us |    2 |       - |       0 B |
     B8000_Enumerator | Core |    Core | 669.379 us | 2.6488 us | 2.2118 us | 666.462 us | 673.755 us | 669.072 us |   12 | 51.1719 |  191526 B |
 B8000_ListEnumerator | Core |    Core |  51.484 us | 0.4148 us | 0.3880 us |  50.987 us |  52.312 us |  51.496 us |    9 |       - |       0 B |
        S8000_Foreach | Core |    Core |  11.170 us | 0.1115 us | 0.0989 us |  11.012 us |  11.352 us |  11.166 us |    5 |       - |       0 B |
            S8000_For | Core |    Core |  11.997 us | 0.1591 us | 0.1488 us |  11.726 us |  12.228 us |  12.053 us |    6 |       - |       0 B |
     S8000_Enumerator | Core |    Core | 214.373 us | 1.2039 us | 1.1261 us | 212.898 us | 216.573 us | 214.492 us |   11 |       - |      36 B |
 S8000_ListEnumerator | Core |    Core |  39.738 us | 0.2751 us | 0.2438 us |  39.491 us |  40.288 us |  39.637 us |    8 |       - |       0 B |
