``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i5-2500K CPU 3.30GHz (Sandy Bridge), 1 CPU, 4 logical cores and 4 physical cores
Frequency=3233540 Hz, Resolution=309.2586 ns, Timer=TSC
  [Host]    : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0
  RyuJitX64 : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2633.0

Job=RyuJitX64  Jit=RyuJit  Platform=X64  

```
|        Method |     Mean |     Error |    StdDev |
|-------------- |---------:|----------:|----------:|
| TestConverAll | 187.0 us | 0.6524 us | 0.5448 us |
