``` ini

BenchmarkDotNet=v0.10.10, OS=Windows 10 Redstone 2 [1703, Creators Update] (10.0.15063.726)
Processor=Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
.NET Core SDK=2.0.3
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  Job-TATWAM : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  Job-FVDTWW : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2115.0
  Clr        : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2115.0
  Core       : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|           Method |     Job | Runtime |     Toolchain |      Mean |     Error |    StdDev |       Min |       Max |    Median | Rank |  Gen 0 | Allocated |
|----------------- |-------- |-------- |-------------- |----------:|----------:|----------:|----------:|----------:|----------:|-----:|-------:|----------:|
| CheckConstructor | Default |    Core | .NET Core 2.0 |  3.619 ns | 0.0810 ns | 0.0633 ns |  3.504 ns |  3.706 ns |  3.622 ns |    2 | 0.0102 |      32 B |
|   CheckActivator | Default |    Core | .NET Core 2.0 | 63.722 ns | 0.9649 ns | 0.9026 ns | 62.794 ns | 65.135 ns | 63.191 ns |    6 | 0.0101 |      32 B |
| CheckConstructor | Default |    Core |   CsProjnet47 |  3.943 ns | 0.1673 ns | 0.2116 ns |  3.660 ns |  4.365 ns |  3.876 ns |    4 | 0.0102 |      32 B |
|   CheckActivator | Default |    Core |   CsProjnet47 | 70.498 ns | 1.2450 ns | 1.1646 ns | 68.252 ns | 72.275 ns | 70.714 ns |    7 | 0.0101 |      32 B |
| CheckConstructor |     Clr |     Clr |       Default |  3.703 ns | 0.0773 ns | 0.0604 ns |  3.623 ns |  3.841 ns |  3.691 ns |    3 | 0.0102 |      32 B |
|   CheckActivator |     Clr |     Clr |       Default | 71.359 ns | 1.4886 ns | 1.6546 ns | 68.545 ns | 74.227 ns | 71.620 ns |    7 | 0.0101 |      32 B |
| CheckConstructor |    Core |    Core |       Default |  3.319 ns | 0.0542 ns | 0.0481 ns |  3.274 ns |  3.432 ns |  3.302 ns |    1 | 0.0102 |      32 B |
|   CheckActivator |    Core |    Core |       Default | 61.132 ns | 1.4662 ns | 1.5688 ns | 59.738 ns | 64.759 ns | 60.493 ns |    5 | 0.0101 |      32 B |
