``` ini

BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233542 Hz, Resolution=309.2584 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  Core   : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |          Method |  Job | Runtime |      Mean |     Error |    StdDev |       Min |       Max |    Median | Rank |  Gen 0 | Allocated |
 |---------------- |----- |-------- |----------:|----------:|----------:|----------:|----------:|----------:|-----:|-------:|----------:|
 |   TestListClass |  Clr |     Clr |  5.599 us | 0.0408 us | 0.0382 us |  5.561 us |  5.689 us |  5.583 us |    3 |      - |       0 B |
 |  TestArrayClass |  Clr |     Clr |  2.024 us | 0.0102 us | 0.0096 us |  2.011 us |  2.043 us |  2.022 us |    2 |      - |       0 B |
 |  TestListStruct |  Clr |     Clr |  8.427 us | 0.1983 us | 0.2204 us |  8.101 us |  9.007 us |  8.374 us |    5 |      - |       0 B |
 | TestArrayStruct |  Clr |     Clr |  1.539 us | 0.0295 us | 0.0276 us |  1.502 us |  1.577 us |  1.537 us |    1 |      - |       0 B |
 |   TestLinqClass |  Clr |     Clr | 13.117 us | 0.1007 us | 0.0892 us | 13.007 us | 13.301 us | 13.089 us |    7 | 0.0153 |      80 B |
 |  TestLinqStruct |  Clr |     Clr | 28.676 us | 0.1837 us | 0.1534 us | 28.441 us | 28.957 us | 28.660 us |    9 |      - |      96 B |
 |   TestListClass | Core |    Core |  5.747 us | 0.1147 us | 0.1275 us |  5.567 us |  5.945 us |  5.756 us |    4 |      - |       0 B |
 |  TestArrayClass | Core |    Core |  2.023 us | 0.0299 us | 0.0279 us |  1.990 us |  2.069 us |  2.013 us |    2 |      - |       0 B |
 |  TestListStruct | Core |    Core |  8.753 us | 0.1659 us | 0.1910 us |  8.498 us |  9.110 us |  8.670 us |    6 |      - |       0 B |
 | TestArrayStruct | Core |    Core |  1.552 us | 0.0307 us | 0.0377 us |  1.496 us |  1.618 us |  1.552 us |    1 |      - |       0 B |
 |   TestLinqClass | Core |    Core | 14.286 us | 0.2430 us | 0.2273 us | 13.956 us | 14.678 us | 14.313 us |    8 | 0.0153 |      72 B |
 |  TestLinqStruct | Core |    Core | 30.121 us | 0.5941 us | 0.5835 us | 28.928 us | 30.909 us | 30.153 us |   10 |      - |      88 B |
