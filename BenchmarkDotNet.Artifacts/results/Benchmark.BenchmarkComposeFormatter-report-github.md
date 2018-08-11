``` ini

BenchmarkDotNet=v0.11.0, OS=Windows 10.0.17134.165 (1803/April2018Update/Redstone4)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Clr    : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT

Toolchain=.NET Core 2.1  

```
|                            Method |  Job | Runtime |        Mean |      Error |     StdDev |         Min |         Max |      Median | Rank |     Gen 0 |   Gen 1 |   Gen 2 |  Allocated |
|---------------------------------- |----- |-------- |------------:|-----------:|-----------:|------------:|------------:|------------:|-----:|----------:|--------:|--------:|-----------:|
|            fastExpressionCompiler |  Clr |     Clr | 11,889.7 us |  35.970 us |  33.646 us | 11,851.1 us | 11,974.4 us | 11,885.4 us |   10 |  625.0000 | 46.8750 | 31.2500 | 2144.31 KB |
|               dslComposeFormatter |  Clr |     Clr |  1,205.7 us |   2.762 us |   2.306 us |  1,202.2 us |  1,211.2 us |  1,205.5 us |    6 |  171.8750 | 42.9688 | 42.9688 |  624.02 KB |
|   dslComposeFormatter_FastCompile |  Clr |     Clr |  1,182.1 us |   6.496 us |   5.424 us |  1,170.6 us |  1,191.7 us |  1,183.0 us |    4 |  171.8750 | 42.9688 | 42.9688 |  624.02 KB |
|                   ImperativeIdeal |  Clr |     Clr |    780.3 us |   2.576 us |   2.284 us |    776.9 us |    785.4 us |    779.5 us |    3 |   85.9375 | 42.9688 | 42.9688 |  281.88 KB |
|                               jil |  Clr |     Clr |    741.5 us |   7.172 us |   6.358 us |    726.2 us |    747.6 us |    744.9 us |    2 |         - |       - |       - |  422.56 KB |
| fake_expressionManuallyConstruted |  Clr |     Clr | 44,525.7 us | 248.962 us | 232.879 us | 44,128.9 us | 44,925.8 us | 44,444.6 us |   13 | 1916.6667 | 83.3333 |       - | 6081.18 KB |
|   fake_delegateManuallyConstruted |  Clr |     Clr |  1,296.3 us |  16.229 us |  15.181 us |  1,278.5 us |  1,328.6 us |  1,293.6 us |    8 |  345.7031 | 64.4531 | 42.9688 | 1224.15 KB |
|            fastExpressionCompiler | Core |    Core | 11,918.4 us |  36.688 us |  34.318 us | 11,874.6 us | 11,995.0 us | 11,913.7 us |   11 |  625.0000 | 46.8750 | 31.2500 | 2144.31 KB |
|               dslComposeFormatter | Core |    Core |  1,217.6 us |   7.458 us |   6.976 us |  1,211.0 us |  1,231.5 us |  1,214.0 us |    7 |  171.8750 | 42.9688 | 42.9688 |  624.02 KB |
|   dslComposeFormatter_FastCompile | Core |    Core |  1,192.6 us |   7.599 us |   7.108 us |  1,183.8 us |  1,207.0 us |  1,190.0 us |    5 |  171.8750 | 42.9688 | 42.9688 |  624.02 KB |
|                   ImperativeIdeal | Core |    Core |    779.6 us |   2.758 us |   2.445 us |    775.6 us |    783.9 us |    779.6 us |    3 |   85.9375 | 42.9688 | 42.9688 |  281.88 KB |
|                               jil | Core |    Core |    726.1 us |  13.785 us |  13.538 us |    708.7 us |    758.0 us |    722.9 us |    1 |         - |       - |       - |  422.56 KB |
| fake_expressionManuallyConstruted | Core |    Core | 43,944.2 us | 123.927 us | 115.922 us | 43,705.7 us | 44,071.8 us | 43,976.3 us |   12 | 1750.0000 |       - |       - | 5668.68 KB |
|   fake_delegateManuallyConstruted | Core |    Core |  1,338.9 us |   8.739 us |   8.174 us |  1,329.1 us |  1,353.8 us |  1,335.6 us |    9 |  345.7031 | 64.4531 | 42.9688 | 1224.15 KB |
