``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Job-OHCUYR : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Job-IEDGXR : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Toolchain=.NET Core 2.1  

```
|                            Method | Runtime |      Mean |     Error |    StdDev |       Min |       Max |    Median | Rank |     Gen 0 |    Gen 1 |   Gen 2 |  Allocated |
|---------------------------------- |-------- |----------:|----------:|----------:|----------:|----------:|----------:|-----:|----------:|---------:|--------:|-----------:|
| fake_expressionManuallyConstruted |     Clr | 54.373 ms | 0.2124 ms | 0.1987 ms | 54.093 ms | 54.722 ms | 54.315 ms |   17 | 2125.0000 | 312.5000 |       - | 7401.58 KB |
|        dslRoutineComposeFormatter |     Clr |  2.187 ms | 0.0128 ms | 0.0120 ms |  2.160 ms |  2.209 ms |  2.188 ms |    1 |  226.5625 |  58.5938 | 58.5938 |  849.47 KB |
|   fake_delegateManuallyConstruted |     Clr |  2.331 ms | 0.0127 ms | 0.0119 ms |  2.311 ms |  2.349 ms |  2.328 ms |    3 |  406.2500 |  78.1250 | 58.5938 | 1494.55 KB |
|                   JsonNet_Default |     Clr |  2.933 ms | 0.0234 ms | 0.0219 ms |  2.906 ms |  2.980 ms |  2.934 ms |    7 |  175.7813 |  58.5938 | 58.5938 |  658.65 KB |
|                           JsonNet |     Clr |  3.084 ms | 0.0120 ms | 0.0107 ms |  3.061 ms |  3.102 ms |  3.083 ms |    8 |  175.7813 |  58.5938 | 58.5938 |  658.95 KB |
|                  JsonNet_Indented |     Clr |  3.842 ms | 0.0197 ms | 0.0184 ms |  3.813 ms |  3.866 ms |  3.844 ms |   13 |  148.4375 |  97.6563 | 97.6563 |  967.25 KB |
|                JsonNet_NullIgnore |     Clr |  2.868 ms | 0.0114 ms | 0.0107 ms |  2.848 ms |  2.886 ms |  2.872 ms |    5 |  136.7188 |  42.9688 | 42.9688 |  564.98 KB |
|              JsonNet_DateFormatFF |     Clr |  3.268 ms | 0.0152 ms | 0.0142 ms |  3.250 ms |  3.297 ms |  3.267 ms |    9 |  214.8438 |  54.6875 | 54.6875 |  757.41 KB |
|              JsonNet_DateFormatSS |     Clr |  3.790 ms | 0.0151 ms | 0.0142 ms |  3.772 ms |  3.827 ms |  3.787 ms |   12 |  175.7813 |  58.5938 | 58.5938 |  785.53 KB |
|    ServiceStack_SerializeToString |     Clr |  4.353 ms | 0.0204 ms | 0.0191 ms |  4.318 ms |  4.384 ms |  4.356 ms |   16 |  218.7500 |  39.0625 | 39.0625 |  805.13 KB |
| fake_expressionManuallyConstruted |    Core | 54.396 ms | 0.1758 ms | 0.1644 ms | 54.104 ms | 54.629 ms | 54.383 ms |   17 | 2125.0000 | 312.5000 |       - | 7401.58 KB |
|        dslRoutineComposeFormatter |    Core |  2.208 ms | 0.0093 ms | 0.0078 ms |  2.193 ms |  2.220 ms |  2.211 ms |    2 |  226.5625 |  58.5938 | 58.5938 |  849.47 KB |
|   fake_delegateManuallyConstruted |    Core |  2.351 ms | 0.0102 ms | 0.0086 ms |  2.334 ms |  2.369 ms |  2.352 ms |    4 |  406.2500 |  78.1250 | 58.5938 | 1494.55 KB |
|                   JsonNet_Default |    Core |  2.902 ms | 0.0160 ms | 0.0150 ms |  2.883 ms |  2.934 ms |  2.899 ms |    6 |  175.7813 |  58.5938 | 58.5938 |  658.63 KB |
|                           JsonNet |    Core |  2.866 ms | 0.0131 ms | 0.0122 ms |  2.844 ms |  2.884 ms |  2.867 ms |    5 |  175.7813 |  58.5938 | 58.5938 |  658.93 KB |
|                  JsonNet_Indented |    Core |  3.508 ms | 0.0145 ms | 0.0136 ms |  3.487 ms |  3.530 ms |  3.510 ms |   11 |  148.4375 |  97.6563 | 97.6563 |  967.23 KB |
|                JsonNet_NullIgnore |    Core |  2.944 ms | 0.0089 ms | 0.0079 ms |  2.932 ms |  2.960 ms |  2.942 ms |    7 |  136.7188 |  42.9688 | 42.9688 |  564.97 KB |
|              JsonNet_DateFormatFF |    Core |  3.480 ms | 0.0121 ms | 0.0113 ms |  3.458 ms |  3.497 ms |  3.479 ms |   10 |  214.8438 |  54.6875 | 54.6875 |  757.41 KB |
|              JsonNet_DateFormatSS |    Core |  3.880 ms | 0.0139 ms | 0.0130 ms |  3.854 ms |  3.899 ms |  3.877 ms |   14 |  175.7813 |  58.5938 | 58.5938 |  785.53 KB |
|    ServiceStack_SerializeToString |    Core |  4.225 ms | 0.0120 ms | 0.0106 ms |  4.201 ms |  4.243 ms |  4.226 ms |   15 |  218.7500 |  39.0625 | 39.0625 |  805.13 KB |
