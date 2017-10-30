using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace Benchmark
{
    [Config(typeof(MyManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkStringBuilderLengthCheck
    {
        StringBuilder sb = new StringBuilder(20000);
        public BenchmarkStringBuilderLengthCheck()
        {

        }
         
        private bool SbAppend1(StringBuilder sb, string text)
        {
            sb.Append(text);
            return text.Length>0;
        }

        private void SbAppend2(StringBuilder sb, string text)
        {
            sb.Append(text);
        }

        [Benchmark]
        public bool CheckWithBool()
        {
            var b = SbAppend1(sb, "x");
            if (b)
            {
                return true;
            }
            return false;
        }

        [Benchmark]
        public bool CheckWithLength()
        {
            var l = sb.Length;
            SbAppend2(sb, "");
            if (sb.Length==l)
            {
                return false;
            }
            return true;
        }
    }
}
