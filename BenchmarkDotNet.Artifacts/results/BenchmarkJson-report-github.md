``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host] : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0
  Clr    : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0


```
|                    Method |     Job | Runtime |     Toolchain |      Mean |     Error |    StdDev |       Min |        Max |    Median | Rank |     Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|-------------------------- |-------- |-------- |-------------- |----------:|----------:|----------:|----------:|-----------:|----------:|-----:|----------:|---------:|--------:|----------:|
|         RoutineExpression | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|         SerializeToString | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
| RoutineExpressionCompiled | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|               RoutineFunc | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|                   JsonNet | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|          JsonNet_Indented | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|        JsonNet_NullIgnore | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|      JsonNet_DateFormatFF | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|      JsonNet_DateFormatSS | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|             ServiceStack1 | Default |    Core | .NET Core 2.0 |        NA |        NA |        NA |        NA |         NA |        NA |    ? |       N/A |      N/A |     N/A |       N/A |
|         RoutineExpression |     Clr |     Clr |       Default | 98.103 ms | 1.6222 ms | 1.5174 ms | 95.406 ms | 100.839 ms | 98.012 ms |    8 | 1000.0000 | 187.5000 |       - | 3769206 B |
|         SerializeToString |     Clr |     Clr |       Default |  3.925 ms | 0.0447 ms | 0.0396 ms |  3.879 ms |   4.014 ms |  3.909 ms |    5 |  152.3438 |  42.9688 | 42.9688 |  565285 B |
| RoutineExpressionCompiled |     Clr |     Clr |       Default |  3.250 ms | 0.0162 ms | 0.0144 ms |  3.220 ms |   3.273 ms |  3.251 ms |    2 |  230.4688 |  58.5938 | 58.5938 |  867516 B |
|               RoutineFunc |     Clr |     Clr |       Default |  3.254 ms | 0.0609 ms | 0.0569 ms |  3.169 ms |   3.356 ms |  3.250 ms |    2 |  292.9688 |  89.8438 | 58.5938 | 1186346 B |
|                   JsonNet |     Clr |     Clr |       Default |  3.456 ms | 0.0691 ms | 0.0848 ms |  3.310 ms |   3.610 ms |  3.477 ms |    3 |  121.0938 |  58.5938 | 58.5938 |  587724 B |
|          JsonNet_Indented |     Clr |     Clr |       Default |  4.211 ms | 0.0830 ms | 0.0853 ms |  4.085 ms |   4.406 ms |  4.221 ms |    6 |  203.1250 |  93.7500 | 93.7500 | 1104542 B |
|        JsonNet_NullIgnore |     Clr |     Clr |       Default |  3.162 ms | 0.0632 ms | 0.1021 ms |  3.039 ms |   3.420 ms |  3.137 ms |    1 |  132.8125 |  42.9688 | 42.9688 |  491688 B |
|      JsonNet_DateFormatFF |     Clr |     Clr |       Default |  4.191 ms | 0.0802 ms | 0.1043 ms |  4.076 ms |   4.425 ms |  4.175 ms |    6 |  210.9375 |  54.6875 | 54.6875 |  808475 B |
|      JsonNet_DateFormatSS |     Clr |     Clr |       Default |  4.314 ms | 0.0670 ms | 0.0626 ms |  4.217 ms |   4.429 ms |  4.308 ms |    7 |  226.5625 |  62.5000 | 54.6875 |  837325 B |
|             ServiceStack1 |     Clr |     Clr |       Default |  3.836 ms | 0.0349 ms | 0.0326 ms |  3.805 ms |   3.912 ms |  3.825 ms |    4 |  148.4375 |  39.0625 | 39.0625 |  565309 B |

Benchmarks with issues:
  BenchmarkJson.RoutineExpression: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.SerializeToString: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.RoutineExpressionCompiled: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.RoutineFunc: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.JsonNet: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.JsonNet_Indented: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.JsonNet_NullIgnore: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.JsonNet_DateFormatFF: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.JsonNet_DateFormatSS: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
  BenchmarkJson.ServiceStack1: Job-JBSQGA(Runtime=Core, Toolchain=.NET Core 2.0)
