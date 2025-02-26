﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;

using BenchmarkDotNet.Attributes;

using DashboardCode.Routines;
using DashboardCode.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(ManualWindowsDiagnosersConfig))]
    [MinColumn, MaxColumn, StdDevColumn, MedianColumn, RankColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
#if NET48_OR_GREATER
    //[HardwareCounters(BenchmarkDotNet.Diagnosers.HardwareCounter.BranchMispredictions, BenchmarkDotNet.Diagnosers.HardwareCounter.BranchInstructions)]
    [DisassemblyDiagnoser(/*printAsm: true,*/ printSource: true)]
    [RyuJitX64Job]
    [BenchmarkDotNet.Diagnostics.Windows.Configs.InliningDiagnoser(logFailuresOnly : true,  filterByNamespace: false)]
#endif
    public class BenchmarkComposeFormatterDisasm
    {
        static readonly Box box;
        static readonly List<Row> testData = new();
        static readonly Func<Box, string> composeFormatterDelegate;
        static readonly Func<StringBuilder, Box, bool> dslRoutineExpressionManuallyConstruted;
        static readonly Func<StringBuilder, Box, bool> dslRoutineDelegateManuallyConstrutedFormatter;
#pragma warning disable CA1810 // Initialize reference type static fields inline
        static BenchmarkComposeFormatterDisasm()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            for (int i = 0; i < 600; i++)
            {
                testData.Add(new Row
                {
                    At = DateTime.Now,
                    I1 = 5,
                    I2 = null,
                    B1 = true,
                    B2 = null,
                    D1 = (decimal)0.21,
                    D2 = (decimal)0.22,
                    D3 = (decimal)0.23,
                    D4 = null,
                    F1 = 0.31,
                    F2 = 0.32,
                    F3 = 0.33,
                    F4 = null
                });
            }
            box = new Box { Rows = testData };

            Include<Box> include = (chain => chain.IncludeAll(e => e.Rows));
            composeFormatterDelegate = JsonManager.ComposeFormatter(include.AppendLeafs(), stringBuilderCapacity: 4000);

            dslRoutineDelegateManuallyConstrutedFormatter = (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sbP, tP,
                        (sb, t) => JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Rows", o => o.Rows,
                            (sb2, t2) => JsonComplexStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sb3, t3,
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "At", o => o.At, JsonValueStringBuilderExtensions.SerializeToIso8601WithMs),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "I1", o => o.I1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "I2", o => o.I2, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "B1", o => o.B1, JsonValueStringBuilderExtensions.SerializeBool),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "B2", o => o.B2, JsonValueStringBuilderExtensions.SerializeBool, JsonValueStringBuilderExtensions.NullSerializer),

                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D1", o => o.D1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D2", o => o.D2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D3", o => o.D3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "D4", o => o.D4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),

                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F1", o => o.F1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F2", o => o.F2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F3", o => o.F3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                     (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "F4", o => o.F4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer)
                                     ),
                                JsonValueStringBuilderExtensions.NullSerializer
                              ),
                            JsonValueStringBuilderExtensions.NullSerializer
                        ));

            Expression<Func<StringBuilder, Box, bool>> dslRoutineExpressionManuallyConstrutedExpression = 
                    (sbP, tP) => JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sbP, tP,
                        (sb, t) => JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull(sb, t, "Rows", o => o.Rows,
                            (sb2, t2) => JsonComplexStringBuilderExtensions.SerializeRefArrayHandleEmpty(sb2, t2,
                                (sb3, t3) =>
                                    JsonComplexStringBuilderExtensions.SerializeAssociativeArray(sb3, t3,
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "At", o => o.At, JsonValueStringBuilderExtensions.SerializeToIso8601WithMs),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "I1", o => o.I1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "I2", o => o.I2, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "B1", o => o.B1, JsonValueStringBuilderExtensions.SerializeBool),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "B2", o => o.B2, JsonValueStringBuilderExtensions.SerializeBool, JsonValueStringBuilderExtensions.NullSerializer),

                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D1", o => o.D1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D2", o => o.D2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "D3", o => o.D3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "D4", o => o.D4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer),

                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F1", o => o.F1, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F2", o => o.F2, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeValueProperty(sb4, t4, "F3", o => o.F3, JsonValueStringBuilderExtensions.SerializeValueToString),
                                        (sb4, t4) => JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull(sb4, t4, "F4", o => o.F4, JsonValueStringBuilderExtensions.SerializeValueToString, JsonValueStringBuilderExtensions.NullSerializer)
                                     ),
                                JsonValueStringBuilderExtensions.NullSerializer
                              ),
                            JsonValueStringBuilderExtensions.NullSerializer
                        )
                    );
            dslRoutineExpressionManuallyConstruted = dslRoutineExpressionManuallyConstrutedExpression.Compile();

            
        }

        [Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public string Fake_expressionManuallyConstruted()
#pragma warning restore CA1822 // Mark members as static
        {
            var sb = new StringBuilder();
            dslRoutineExpressionManuallyConstruted(sb, box);
            var json = sb.ToString();
            return json;
        }


        [Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public string DslComposeFormatter()
#pragma warning restore CA1822 // Mark members as static
        {
            var json = composeFormatterDelegate(box);
            return json;
        }


        [Benchmark]
#pragma warning disable CA1822 // Mark members as static
        public string Fake_delegateManuallyConstruted()
#pragma warning restore CA1822 // Mark members as static
        {
            var sb = new StringBuilder();
            dslRoutineDelegateManuallyConstrutedFormatter(sb, box);
            var json = sb.ToString();
            return json;
        }
    }
}