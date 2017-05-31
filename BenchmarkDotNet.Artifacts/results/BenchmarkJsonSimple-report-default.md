
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0

Job=Clr  Runtime=Clr  

                Method |       Mean |     Error |    StdDev |     Median |        Min |        Max | Rank |  Gen 0 | Allocated |
---------------------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-----:|-------:|----------:|
              TestFunc |   770.7 ns | 23.230 ns | 68.493 ns |   744.9 ns |   682.5 ns |   969.7 ns |    2 | 2.5903 |      8 kB |
       TestFuncBuilded |   698.9 ns |  9.877 ns |  8.248 ns |   702.7 ns |   687.7 ns |   709.2 ns |    1 | 2.5706 |   7.94 kB |
 TestFuncDynamicInvoke | 2,507.7 ns | 24.669 ns | 21.868 ns | 2,508.9 ns | 2,473.8 ns | 2,542.3 ns |    3 | 2.6245 |   8.24 kB |
