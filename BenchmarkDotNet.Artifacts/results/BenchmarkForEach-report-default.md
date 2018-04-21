
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.16299.371 (1709/FallCreatorsUpdate/Redstone3)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical and 4 physical cores
Frequency=3233541 Hz, Resolution=309.2585 ns, Timer=TSC
.NET Core SDK=2.1.100
  [Host]     : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Job-JGYWSB : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT
  Job-XVJYLH : .NET Core 2.0.5 (CoreCLR 4.6.26020.03, CoreFX 4.6.26018.01), 64bit RyuJIT

Toolchain=.NET Core 2.0  

                      Method | Runtime |      Mean |     Error |    StdDev |       Min |       Max |    Median | Rank | Allocated |
---------------------------- |-------- |----------:|----------:|----------:|----------:|----------:|----------:|-----:|----------:|
  TestListCacheTmpEnumerable |     Clr | 80.290 us | 1.5359 us | 1.8284 us | 77.341 us | 84.398 us | 80.290 us |    7 |      40 B |
            TestListCacheTmp |     Clr | 26.557 us | 0.4275 us | 0.3790 us | 25.914 us | 27.056 us | 26.710 us |    4 |       0 B |
              TestListInCall |     Clr | 26.829 us | 0.5337 us | 0.5481 us | 26.026 us | 27.773 us | 26.787 us |    4 |       0 B |
                    TestList |     Clr | 26.781 us | 0.3702 us | 0.3463 us | 26.106 us | 27.242 us | 26.806 us |    4 |       0 B |
 TestArrayCacheTmpEnumerable |     Clr | 66.198 us | 0.7495 us | 0.7011 us | 65.248 us | 67.488 us | 66.121 us |    5 |      32 B |
           TestArrayCacheTmp |     Clr |  6.076 us | 0.1268 us | 0.1649 us |  5.907 us |  6.507 us |  6.016 us |    2 |       0 B |
             TestArrayInCall |     Clr |  5.988 us | 0.0518 us | 0.0484 us |  5.925 us |  6.078 us |  5.968 us |    1 |       0 B |
                   TestArray |     Clr |  5.950 us | 0.0442 us | 0.0414 us |  5.898 us |  6.034 us |  5.942 us |    1 |       0 B |
  TestListCacheTmpEnumerable |    Core | 77.668 us | 0.8426 us | 0.7882 us | 76.812 us | 79.128 us | 77.298 us |    6 |      40 B |
            TestListCacheTmp |    Core | 26.334 us | 0.3406 us | 0.3186 us | 25.912 us | 27.006 us | 26.255 us |    4 |       0 B |
              TestListInCall |    Core | 26.025 us | 0.0972 us | 0.0862 us | 25.849 us | 26.197 us | 26.044 us |    3 |       0 B |
                    TestList |    Core | 25.997 us | 0.2040 us | 0.1908 us | 25.734 us | 26.392 us | 26.034 us |    3 |       0 B |
 TestArrayCacheTmpEnumerable |    Core | 65.822 us | 1.0809 us | 1.0111 us | 64.800 us | 68.408 us | 65.538 us |    5 |      32 B |
           TestArrayCacheTmp |    Core |  5.958 us | 0.0540 us | 0.0505 us |  5.871 us |  6.048 us |  5.959 us |    1 |       0 B |
             TestArrayInCall |    Core |  5.938 us | 0.0320 us | 0.0299 us |  5.883 us |  5.998 us |  5.934 us |    1 |       0 B |
                   TestArray |    Core |  5.936 us | 0.0405 us | 0.0338 us |  5.885 us |  6.018 us |  5.935 us |    1 |       0 B |
