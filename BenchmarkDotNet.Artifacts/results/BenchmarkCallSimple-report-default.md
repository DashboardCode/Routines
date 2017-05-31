
BenchmarkDotNet=v0.10.5, OS=Windows 10.0.14393
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233539 Hz, Resolution=309.2587 ns, Timer=TSC
  [Host] : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Clr    : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1648.0
  Core   : .NET Core 4.6.25211.01, 64bit RyuJIT


          Method |  Job | Runtime |     Mean |     Error |    StdDev |      Min |      Max |   Median | Rank |  Gen 0 | Allocated |
---------------- |----- |-------- |---------:|----------:|----------:|---------:|---------:|---------:|-----:|-------:|----------:|
 CallBuildedReal |  Clr |     Clr | 138.5 ns | 2.4653 ns | 2.3060 ns | 134.6 ns | 141.8 ns | 139.0 ns |    5 | 0.0577 |     192 B |
     CallBuilded |  Clr |     Clr | 122.3 ns | 1.7136 ns | 1.6029 ns | 120.1 ns | 124.9 ns | 122.4 ns |    4 | 0.0576 |     192 B |
      CallLambda |  Clr |     Clr | 122.7 ns | 1.5679 ns | 1.4666 ns | 120.5 ns | 125.8 ns | 122.6 ns |    4 | 0.0582 |     192 B |
 CallLambdaConst |  Clr |     Clr | 122.7 ns | 0.7203 ns | 0.6385 ns | 121.6 ns | 123.8 ns | 122.6 ns |    4 | 0.0577 |     192 B |
 CallBuildedReal | Core |    Core | 121.6 ns | 1.7688 ns | 1.4770 ns | 119.2 ns | 124.0 ns | 121.9 ns |    4 | 0.0577 |     192 B |
     CallBuilded | Core |    Core | 110.0 ns | 1.0281 ns | 0.8585 ns | 108.9 ns | 111.7 ns | 109.9 ns |    1 | 0.0593 |     192 B |
      CallLambda | Core |    Core | 112.3 ns | 1.6818 ns | 1.4909 ns | 110.2 ns | 115.1 ns | 112.4 ns |    2 | 0.0592 |     192 B |
 CallLambdaConst | Core |    Core | 115.9 ns | 2.2959 ns | 2.1475 ns | 112.3 ns | 119.3 ns | 115.0 ns |    3 | 0.0594 |     192 B |
