```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.5371/22H2/2022Update)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX
  Job-JZRVEH : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX


```
| Method                                               | Runtime            | Mean        | Error     | StdDev    | Median      | Min         | Max         | Rank | Gen0      | Gen1     | Gen2    | Allocated  |
|----------------------------------------------------- |------------------- |------------:|----------:|----------:|------------:|------------:|------------:|-----:|----------:|---------:|--------:|-----------:|
| dslComposeFormatter_NoInnerLambdaCompile             | .NET 9.0           | 17,512.0 μs | 284.93 μs | 266.52 μs | 17,578.1 μs | 17,058.6 μs | 17,927.1 μs |    9 |  500.0000 |  93.7500 | 31.2500 | 1888.54 KB |
| dslComposeFormatter_NoInnerLambdaCompile_FastCompile | .NET 9.0           |  1,008.1 μs |  18.54 μs |  41.47 μs |    989.0 μs |    967.7 μs |  1,112.0 μs |    2 |  175.7813 |  58.5938 | 58.5938 |  669.44 KB |
| dslComposeFormatter                                  | .NET 9.0           |  1,065.5 μs |  20.87 μs |  28.56 μs |  1,060.3 μs |  1,024.1 μs |  1,129.2 μs |    2 |  175.7813 |  58.5938 | 58.5938 |  669.44 KB |
| dslComposeFormatter_FastCompile                      | .NET 9.0           |  1,013.3 μs |  12.70 μs |  10.61 μs |  1,012.3 μs |  1,002.6 μs |  1,041.0 μs |    2 |  175.7813 |  58.5938 | 58.5938 |  669.44 KB |
| Manually_ImperativeIdeal                             | .NET 9.0           |    725.0 μs |   6.36 μs |   5.64 μs |    725.0 μs |    717.2 μs |    737.1 μs |    1 |  117.1875 |  58.5938 | 58.5938 |  420.34 KB |
| Manually_FunctionalIdeal                             | .NET 9.0           |  1,306.3 μs |  26.07 μs |  59.38 μs |  1,288.5 μs |  1,219.1 μs |  1,437.9 μs |    3 |  351.5625 |  60.5469 | 58.5938 | 1250.82 KB |
| Fake_expression_CompileFast                          | .NET 9.0           |  9,007.6 μs |  39.34 μs |  36.80 μs |  9,019.4 μs |  8,945.8 μs |  9,065.3 μs |    8 |  312.5000 |  78.1250 | 46.8750 | 1250.81 KB |
| Fake_expression_Compile                              | .NET 9.0           | 30,519.3 μs | 350.86 μs | 328.20 μs | 30,520.3 μs | 30,143.2 μs | 31,309.8 μs |   10 | 1218.7500 | 187.5000 | 31.2500 | 4035.72 KB |
| JsonNet_Default                                      | .NET 9.0           |  1,644.4 μs |  11.00 μs |   9.75 μs |  1,645.8 μs |  1,624.6 μs |  1,658.6 μs |    5 |  175.7813 |  58.5938 | 58.5938 |  630.52 KB |
| JsonNet_NullIgnore                                   | .NET 9.0           |  1,546.2 μs |   9.05 μs |   8.02 μs |  1,545.4 μs |  1,530.3 μs |  1,562.0 μs |    4 |  134.7656 |  44.9219 | 44.9219 |  536.84 KB |
| JsonNet_DateFormatMMSS                               | .NET 9.0           |  1,676.1 μs |  32.41 μs |  36.02 μs |  1,681.1 μs |  1,606.6 μs |  1,735.0 μs |    5 |  164.0625 |  54.6875 | 54.6875 |  663.66 KB |
| JsonNet_DateFormatMMSSFFF                            | .NET 9.0           |  1,776.1 μs |  16.71 μs |  13.96 μs |  1,774.7 μs |  1,754.3 μs |  1,805.6 μs |    6 |  175.7813 |  58.5938 | 58.5938 |  673.04 KB |
| JsonNet_Indented                                     | .NET 9.0           |  2,006.9 μs |  39.47 μs |  36.92 μs |  2,016.6 μs |  1,950.1 μs |  2,064.9 μs |    7 |  121.0938 | 117.1875 | 97.6563 |  939.15 KB |
| ServiceStack_SerializeToString                       | .NET 9.0           |  1,770.9 μs |  26.90 μs |  25.17 μs |  1,771.6 μs |  1,740.6 μs |  1,819.5 μs |    6 |  171.8750 |  42.9688 | 42.9688 |  585.58 KB |
| dslComposeFormatter_NoInnerLambdaCompile             | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| dslComposeFormatter_NoInnerLambdaCompile_FastCompile | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| dslComposeFormatter                                  | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| dslComposeFormatter_FastCompile                      | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| Manually_ImperativeIdeal                             | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| Manually_FunctionalIdeal                             | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| Fake_expression_CompileFast                          | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| Fake_expression_Compile                              | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| JsonNet_Default                                      | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| JsonNet_NullIgnore                                   | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| JsonNet_DateFormatMMSS                               | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| JsonNet_DateFormatMMSSFFF                            | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| JsonNet_Indented                                     | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |
| ServiceStack_SerializeToString                       | .NET Framework 4.8 |          NA |        NA |        NA |          NA |          NA |          NA |    ? |        NA |       NA |      NA |         NA |

Benchmarks with issues:
  BenchmarkComposeFormatter.dslComposeFormatter_NoInnerLambdaCompile: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.dslComposeFormatter_NoInnerLambdaCompile_FastCompile: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.dslComposeFormatter: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.dslComposeFormatter_FastCompile: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.Manually_ImperativeIdeal: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.Manually_FunctionalIdeal: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.Fake_expression_CompileFast: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.Fake_expression_Compile: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.JsonNet_Default: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.JsonNet_NullIgnore: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.JsonNet_DateFormatMMSS: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.JsonNet_DateFormatMMSSFFF: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.JsonNet_Indented: Job-HCRJTJ(Runtime=.NET Framework 4.8)
  BenchmarkComposeFormatter.ServiceStack_SerializeToString: Job-HCRJTJ(Runtime=.NET Framework 4.8)
