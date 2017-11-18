
BenchmarkDotNet=v0.10.10, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233538 Hz, Resolution=309.2588 ns, Timer=TSC
  [Host] : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2115.0
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2115.0

Job=Clr  Runtime=Clr  

        Method |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |   Gen 0 |  Gen 1 | Allocated |
-------------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|--------:|-------:|----------:|
 TestConverAll | 194.6 us | 3.4859 us | 3.2607 us | 188.8 us | 199.6 us | 194.5 us |    1 | 17.5781 | 0.2441 |  54.72 KB |
    TestSelect | 197.2 us | 0.9992 us | 0.9347 us | 196.0 us | 199.3 us | 197.0 us |    2 | 17.5781 | 0.2441 |  54.72 KB |
