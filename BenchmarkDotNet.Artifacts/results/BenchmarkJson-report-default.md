
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1648.0

Job=Clr  Runtime=Clr  

                    Method |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |    Gen 0 |   Gen 1 |   Gen 2 |  Allocated |
-------------------------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|---------:|--------:|--------:|-----------:|
 RoutineExpressionCompiled | 3.269 ms | 0.0241 ms | 0.0201 ms | 3.242 ms | 3.313 ms | 3.269 ms |    2 | 196.8750 | 21.0938 | 21.0938 |  840.05 kB |
               RoutineFunc | 3.085 ms | 0.0179 ms | 0.0168 ms | 3.059 ms | 3.120 ms | 3.079 ms |    1 | 256.5104 | 53.3854 | 22.1354 | 1158.57 kB |
                   JsonNet | 3.371 ms | 0.0307 ms | 0.0272 ms | 3.320 ms | 3.426 ms | 3.368 ms |    3 |  80.4688 | 17.9688 | 17.9688 |  573.97 kB |
             ServiceStack1 | 3.970 ms | 0.0648 ms | 0.0606 ms | 3.888 ms | 4.093 ms | 3.962 ms |    4 |  71.3542 |       - |       - |  552.11 kB |
