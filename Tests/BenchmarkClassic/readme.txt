Use this to utilize "Windows Only" BenchmarkDotNet diagnosers
   JIT Inlining Events (InliningDiagnoser);
   JIT Tail Call Events (TailCallDiagnoser);
   Hardware Counter Diagnoser;
   Disassembly Diagnoser (it can be utilized in BenchmarkDotNet core project but can be used only on Windows)

So do not use it for performance comparisions with Core. [ClrJob] only!
