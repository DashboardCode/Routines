``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Job-UYDYHW : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Job-LLCKPY : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Toolchain=.NET Core 2.1  

```
|                            Method | Runtime |      Mean |     Error |    StdDev |       Min |       Max |    Median | Rank |     Gen 0 |    Gen 1 |   Gen 2 |  Allocated |
|---------------------------------- |-------- |----------:|----------:|----------:|----------:|----------:|----------:|-----:|----------:|---------:|--------:|-----------:|
|            fastExpressionCompiler |     Clr | 15.075 ms | 0.0490 ms | 0.0458 ms | 15.002 ms | 15.143 ms | 15.059 ms |    9 |  625.0000 | 171.8750 | 46.8750 | 2489.78 KB |
|               dslComposeFormatter |     Clr |  2.291 ms | 0.0245 ms | 0.0229 ms |  2.269 ms |  2.333 ms |  2.284 ms |    6 |  226.5625 |  62.5000 | 58.5938 |  856.92 KB |
|   dslComposeFormatter_FastCompile |     Clr |  2.264 ms | 0.0175 ms | 0.0164 ms |  2.244 ms |  2.297 ms |  2.260 ms |    5 |  226.5625 |  62.5000 | 58.5938 |  856.92 KB |
|                               jil |     Clr |  1.113 ms | 0.0118 ms | 0.0110 ms |  1.103 ms |  1.138 ms |  1.108 ms |    2 |  109.3750 |  54.6875 | 54.6875 |  505.95 KB |
| fake_expressionManuallyConstruted |     Clr | 51.068 ms | 0.1025 ms | 0.0959 ms | 50.906 ms | 51.214 ms | 51.088 ms |   10 | 1687.5000 | 437.5000 |       - | 6164.08 KB |
|   fake_delegateManuallyConstruted |     Clr |  2.362 ms | 0.0092 ms | 0.0086 ms |  2.349 ms |  2.382 ms |  2.359 ms |    7 |  406.2500 |  78.1250 | 58.5938 | 1494.55 KB |
|            fastExpressionCompiler |    Core | 14.751 ms | 0.0313 ms | 0.0293 ms | 14.715 ms | 14.821 ms | 14.745 ms |    8 |  625.0000 | 171.8750 | 46.8750 | 2489.78 KB |
|               dslComposeFormatter |    Core |  2.216 ms | 0.0052 ms | 0.0049 ms |  2.207 ms |  2.222 ms |  2.218 ms |    3 |  226.5625 |  62.5000 | 58.5938 |  856.92 KB |
|   dslComposeFormatter_FastCompile |    Core |  2.225 ms | 0.0085 ms | 0.0079 ms |  2.212 ms |  2.237 ms |  2.225 ms |    4 |  226.5625 |  62.5000 | 58.5938 |  856.92 KB |
|                               jil |    Core |  1.105 ms | 0.0031 ms | 0.0027 ms |  1.100 ms |  1.108 ms |  1.105 ms |    1 |  109.3750 |  54.6875 | 54.6875 |  505.95 KB |
| fake_expressionManuallyConstruted |    Core | 51.053 ms | 0.2729 ms | 0.2553 ms | 50.739 ms | 51.523 ms | 51.066 ms |   10 | 1687.5000 | 437.5000 |       - | 6164.08 KB |
|   fake_delegateManuallyConstruted |    Core |  2.362 ms | 0.0118 ms | 0.0105 ms |  2.341 ms |  2.382 ms |  2.363 ms |    7 |  406.2500 |  78.1250 | 58.5938 | 1494.55 KB |
