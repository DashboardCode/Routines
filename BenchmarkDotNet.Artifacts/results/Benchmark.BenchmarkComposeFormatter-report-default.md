
BenchmarkDotNet=v0.11.0, OS=Windows 10.0.17134.165 (1803/April2018Update/Redstone4)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host] : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Clr    : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3131.0
  Core   : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


                                               Method |  Job | Runtime |     Toolchain |       Mean |     Error |    StdDev |         Min |        Max |     Median | Rank |     Gen 0 |    Gen 1 |   Gen 2 |  Allocated |
----------------------------------------------------- |----- |-------- |-------------- |-----------:|----------:|----------:|------------:|-----------:|-----------:|-----:|----------:|---------:|--------:|-----------:|
             dslComposeFormatter_NoInnerLambdaCompile |  Clr |     Clr |       Default |  27.391 ms | 0.1350 ms | 0.1197 ms |  27.1759 ms |  27.660 ms |  27.388 ms |   27 |  593.7500 |  93.7500 | 31.2500 | 2315.25 KB |
 dslComposeFormatter_NoInnerLambdaCompile_FastCompile |  Clr |     Clr |       Default |  27.471 ms | 0.0802 ms | 0.0750 ms |  27.3426 ms |  27.607 ms |  27.461 ms |   28 |  593.7500 |  93.7500 | 31.2500 | 2315.25 KB |
                                  dslComposeFormatter |  Clr |     Clr |       Default |   3.505 ms | 0.0132 ms | 0.0124 ms |   3.4847 ms |   3.526 ms |   3.506 ms |   15 |  292.9688 |  62.5000 | 58.5938 | 1096.25 KB |
                      dslComposeFormatter_FastCompile |  Clr |     Clr |       Default |   3.209 ms | 0.0175 ms | 0.0164 ms |   3.1896 ms |   3.244 ms |   3.203 ms |   11 |  292.9688 |  62.5000 | 58.5938 | 1096.25 KB |
                             Manually_ImperativeIdeal |  Clr |     Clr |       Default |   2.508 ms | 0.0146 ms | 0.0136 ms |   2.4912 ms |   2.531 ms |   2.505 ms |    8 |  234.3750 |  62.5000 | 58.5938 |   903.3 KB |
                             Manually_FunctionalIdeal |  Clr |     Clr |       Default |   3.340 ms | 0.0178 ms | 0.0167 ms |   3.3218 ms |   3.365 ms |   3.333 ms |   12 |  449.2188 | 101.5625 | 58.5938 | 1733.72 KB |
                          Fake_expression_CompileFast |  Clr |     Clr |       Default |  48.247 ms | 0.2980 ms | 0.2788 ms |  47.7302 ms |  48.627 ms |  48.309 ms |   29 |  636.3636 | 181.8182 |       - | 2729.22 KB |
                              Fake_expression_Compile |  Clr |     Clr |       Default | 103.414 ms | 0.6166 ms | 0.5466 ms | 102.5918 ms | 104.591 ms | 103.252 ms |   31 | 2000.0000 | 200.0000 |       - |  7107.5 KB |
                                      JsonNet_Default |  Clr |     Clr |       Default |   3.629 ms | 0.0192 ms | 0.0170 ms |   3.6068 ms |   3.659 ms |   3.627 ms |   17 |  175.7813 |  58.5938 | 58.5938 |  659.09 KB |
                                   JsonNet_NullIgnore |  Clr |     Clr |       Default |   3.462 ms | 0.0168 ms | 0.0140 ms |   3.4351 ms |   3.481 ms |   3.465 ms |   14 |  136.7188 |  42.9688 | 42.9688 |  565.24 KB |
                               JsonNet_DateFormatMMSS |  Clr |     Clr |       Default |   4.885 ms | 0.0509 ms | 0.0476 ms |   4.8120 ms |   4.991 ms |   4.873 ms |   22 |  273.4375 |  54.6875 | 54.6875 |   996.7 KB |
                            JsonNet_DateFormatMMSSFFF |  Clr |     Clr |       Default |   5.049 ms | 0.0573 ms | 0.0536 ms |   4.9990 ms |   5.186 ms |   5.027 ms |   23 |  281.2500 |  54.6875 | 54.6875 | 1024.82 KB |
                                     JsonNet_Indented |  Clr |     Clr |       Default |   4.392 ms | 0.0389 ms | 0.0345 ms |   4.3573 ms |   4.491 ms |   4.386 ms |   21 |  210.9375 |  93.7500 | 93.7500 | 1249.02 KB |
                       ServiceStack_SerializeToString |  Clr |     Clr |       Default |   3.702 ms | 0.0139 ms | 0.0130 ms |   3.6844 ms |   3.726 ms |   3.700 ms |   18 |  214.8438 |  42.9688 | 42.9688 |  734.85 KB |
                                     jil_excludeNulls |  Clr |     Clr |       Default |   1.364 ms | 0.0124 ms | 0.0097 ms |   1.3464 ms |   1.384 ms |   1.364 ms |    3 |         - |        - |       - |  414.38 KB |
                                                  jil |  Clr |     Clr |       Default |   1.397 ms | 0.0257 ms | 0.0240 ms |   1.3685 ms |   1.452 ms |   1.391 ms |    4 |         - |        - |       - |  508.27 KB |
             dslComposeFormatter_NoInnerLambdaCompile | Core |    Core | .NET Core 2.1 |  21.785 ms | 0.0806 ms | 0.0754 ms |  21.6714 ms |  21.906 ms |  21.805 ms |   25 |  531.2500 | 156.2500 | 31.2500 | 2076.02 KB |
 dslComposeFormatter_NoInnerLambdaCompile_FastCompile | Core |    Core | .NET Core 2.1 |  21.864 ms | 0.0718 ms | 0.0671 ms |  21.7825 ms |  21.983 ms |  21.851 ms |   26 |  531.2500 | 156.2500 | 31.2500 | 2076.02 KB |
                                  dslComposeFormatter | Core |    Core | .NET Core 2.1 |   2.186 ms | 0.0097 ms | 0.0091 ms |   2.1723 ms |   2.202 ms |   2.186 ms |    6 |  226.5625 |  62.5000 | 58.5938 |  856.92 KB |
                      dslComposeFormatter_FastCompile | Core |    Core | .NET Core 2.1 |   2.184 ms | 0.0070 ms | 0.0062 ms |   2.1744 ms |   2.194 ms |   2.183 ms |    6 |  226.5625 |  62.5000 | 58.5938 |  856.92 KB |
                             Manually_ImperativeIdeal | Core |    Core | .NET Core 2.1 |   1.558 ms | 0.0082 ms | 0.0073 ms |   1.5424 ms |   1.569 ms |   1.558 ms |    5 |  117.1875 |  58.5938 | 58.5938 |  504.71 KB |
                             Manually_FunctionalIdeal | Core |    Core | .NET Core 2.1 |   2.358 ms | 0.0134 ms | 0.0111 ms |   2.3428 ms |   2.384 ms |   2.360 ms |    7 |  406.2500 |  78.1250 | 58.5938 | 1494.55 KB |
                          Fake_expression_CompileFast | Core |    Core | .NET Core 2.1 |  14.615 ms | 0.0404 ms | 0.0377 ms |  14.5629 ms |  14.686 ms |  14.613 ms |   24 |  625.0000 | 171.8750 | 46.8750 | 2489.78 KB |
                              Fake_expression_Compile | Core |    Core | .NET Core 2.1 |  50.226 ms | 0.2860 ms | 0.2676 ms |  49.6023 ms |  50.650 ms |  50.252 ms |   30 | 1700.0000 | 400.0000 |       - | 6164.08 KB |
                                      JsonNet_Default | Core |    Core | .NET Core 2.1 |   2.934 ms | 0.0104 ms | 0.0097 ms |   2.9209 ms |   2.955 ms |   2.930 ms |   10 |  175.7813 |  58.5938 | 58.5938 |  658.63 KB |
                                   JsonNet_NullIgnore | Core |    Core | .NET Core 2.1 |   2.753 ms | 0.0088 ms | 0.0083 ms |   2.7408 ms |   2.771 ms |   2.752 ms |    9 |  136.7188 |  42.9688 | 42.9688 |  564.97 KB |
                               JsonNet_DateFormatMMSS | Core |    Core | .NET Core 2.1 |   3.360 ms | 0.0154 ms | 0.0144 ms |   3.3418 ms |   3.398 ms |   3.355 ms |   13 |  214.8438 |  54.6875 | 54.6875 |  757.41 KB |
                            JsonNet_DateFormatMMSSFFF | Core |    Core | .NET Core 2.1 |   3.775 ms | 0.0105 ms | 0.0098 ms |   3.7616 ms |   3.794 ms |   3.771 ms |   19 |  175.7813 |  58.5938 | 58.5938 |  785.53 KB |
                                     JsonNet_Indented | Core |    Core | .NET Core 2.1 |   3.570 ms | 0.0170 ms | 0.0159 ms |   3.5462 ms |   3.601 ms |   3.567 ms |   16 |  148.4375 |  97.6563 | 97.6563 |  967.24 KB |
                       ServiceStack_SerializeToString | Core |    Core | .NET Core 2.1 |   3.884 ms | 0.0267 ms | 0.0250 ms |   3.8563 ms |   3.928 ms |   3.873 ms |   20 |  218.7500 |  42.9688 | 42.9688 |  805.13 KB |
                                     jil_excludeNulls | Core |    Core | .NET Core 2.1 |   1.008 ms | 0.0163 ms | 0.0152 ms |   0.9901 ms |   1.038 ms |   1.002 ms |    1 |         - |        - |       - |  412.02 KB |
                                                  jil | Core |    Core | .NET Core 2.1 |   1.032 ms | 0.0120 ms | 0.0100 ms |   1.0191 ms |   1.054 ms |   1.031 ms |    2 |         - |        - |       - |  505.98 KB |
