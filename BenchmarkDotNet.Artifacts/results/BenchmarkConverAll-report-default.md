
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233542 Hz, Resolution=309.2584 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.7.2101.1
  Clr    : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.7.2101.1

Job=Clr  Runtime=Clr  

        Method |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |   Gen 0 |  Gen 1 | Allocated |
-------------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|--------:|-------:|----------:|
 TestConverAll | 192.7 us | 1.6772 us | 1.5689 us | 190.3 us | 195.4 us | 192.4 us |    2 | 17.5781 | 0.2441 |  54.72 KB |
    TestSelect | 191.8 us | 0.7062 us | 0.6606 us | 190.3 us | 192.6 us | 191.9 us |    1 | 17.5781 | 0.2441 |  54.72 KB |
