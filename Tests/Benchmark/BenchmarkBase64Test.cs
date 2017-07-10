using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Collections.Generic;
using Vse.Routines;
using Vse.Routines.Json;

namespace Benchmark
{
    //[Config(typeof(Config))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [ClrJob, CoreJob]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkBase64Test
    {
        static byte[] bytes8000;
        static byte[] bytes64;
        static BenchmarkBase64Test()
        {
            var rnd = new Random();
            var b1 = new List<byte>();
            for(int i=0;i<8002;i++)
            {
                b1.Add((byte)rnd.Next(0,255));
            }
            bytes8000 = b1.ToArray();

            var b2 = new List<byte>();
            for (int i = 0; i < 64; i++)
            {
                b2.Add((byte)rnd.Next(0, 255));
            }
            bytes64 = b2.ToArray();
        }

        [Benchmark]
        public string Custom8000()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBase64Custom(sb, bytes8000);
            var text = sb.ToString();
            return text;
        }

        //[Benchmark]
        public string Custom64()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBase64Custom(sb, bytes64);
            var text = sb.ToString();
            return text;
        }

        [Benchmark]
        public string Convert8000()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBase64(sb, bytes8000);
            var text = sb.ToString();
            return text;
        }

        //[Benchmark]
        public string Convert64()
        {
            var sb = new StringBuilder();
            JsonValueStringBuilderExtensions.SerializeBase64(sb, bytes64);
            var text = sb.ToString();
            return text;
        }

        //[Benchmark]
        public byte[] AmitBens64()
        {
            var r = AmitBensAppendBase64(bytes64, 0, bytes64.Length, false);
            return r;
        }

        //[Benchmark]
        public byte[] AmitBens8000()
        {
            var r = AmitBensAppendBase64(bytes8000, 0, bytes8000.Length, false);
            return r;
        }

        //[Benchmark]
        public string AmitBens64ToEncoding()
        {
            var r = AmitBensAppendBase64(bytes64, 0, bytes64.Length, false);
            return Encoding.ASCII.GetString(r); 
        }

        //[Benchmark]
        public string AmitBens8000ToEncoding()
        {
            var r = AmitBensAppendBase64(bytes8000, 0, bytes8000.Length, false);
            return Encoding.ASCII.GetString(r);
        }

        //[Benchmark]
        public string AmitBens64ToStringBuilder()
        {
            var r = AmitBensAppendBase64(bytes64, 0, bytes64.Length, false);
            var sb = new StringBuilder(r.Length);
            foreach (var b in r)
                sb.Append((char)b);
            return sb.ToString();
        }

        //[Benchmark]
        public string AmitBens8000ToStringBuilder()
        {
            var r = AmitBensAppendBase64(bytes8000, 0, bytes8000.Length, false);
            var sb = new StringBuilder(r.Length);
            foreach (var b in r)
                sb.Append((char)b);
            return sb.ToString();
        }

        //[Benchmark]
        public string AmitBens64ToString()
        {
            var r = AmitBensAppendBase64(bytes64, 0, bytes64.Length, false);
            var sb = new char[r.Length];
            for (int i=0; i<r.Length; i++)
                sb[i] = (char)(r[i]);
            return new string(sb);
        }

        //[Benchmark]
        public string AmitBens8000ToString()
        {
            var r = AmitBensAppendBase64(bytes8000, 0, bytes8000.Length, false);
            var sb = new char[r.Length];
            for (int i = 0; i < r.Length; i++)
                sb[i] = (char)(r[i]);
            return new string(sb);
        }

        static byte[] base64EncodingTable = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/");

        // http://stackoverflow.com/questions/12178495/an-efficient-way-to-base64-encode-a-byte-array
        public byte[] AmitBensAppendBase64(byte[] data
                                  , int offset
                                  , int size
                                  , bool addLineBreaks = false)
        {
            byte[] buffer;
            int requiredSize = (4 * ((size + 2) / 3));
            // size/76*2 for 2 line break characters    
            if (addLineBreaks) requiredSize += requiredSize + (requiredSize / 38);

            buffer = new byte[requiredSize];

            UInt32 octet_a;
            UInt32 octet_b;
            UInt32 octet_c;
            UInt32 triple;
            int lineCount = 0;
            int sizeMod = size - (size % 3);
            var mBufferPos = 0;
            // adding all data triplets
            for (; offset < sizeMod;)
            {
                octet_a = data[offset++];
                octet_b = data[offset++];
                octet_c = data[offset++];

                triple = (octet_a << 0x10) + (octet_b << 0x08) + octet_c;

                buffer[mBufferPos++] = base64EncodingTable[(triple >> 3 * 6) & 0x3F];
                buffer[mBufferPos++] = base64EncodingTable[(triple >> 2 * 6) & 0x3F];
                buffer[mBufferPos++] = base64EncodingTable[(triple >> 1 * 6) & 0x3F];
                buffer[mBufferPos++] = base64EncodingTable[(triple >> 0 * 6) & 0x3F];
                if (addLineBreaks)
                {
                    if (++lineCount == 19)
                    {
                        buffer[mBufferPos++] = 13;
                        buffer[mBufferPos++] = 10;
                        lineCount = 0;
                    }
                }
            }

            // last bytes
            if (sizeMod < size)
            {
                octet_a = offset < size ? data[offset++] : (UInt32)0;
                octet_b = offset < size ? data[offset++] : (UInt32)0;
                octet_c = (UInt32)0; // last character is definitely padded

                triple = (octet_a << 0x10) + (octet_b << 0x08) + octet_c;

                buffer[mBufferPos++] = base64EncodingTable[(triple >> 3 * 6) & 0x3F];
                buffer[mBufferPos++] = base64EncodingTable[(triple >> 2 * 6) & 0x3F];
                buffer[mBufferPos++] = base64EncodingTable[(triple >> 1 * 6) & 0x3F];
                buffer[mBufferPos++] = base64EncodingTable[(triple >> 0 * 6) & 0x3F];

                // add padding '='
                sizeMod = size % 3;
                // last character is definitely padded
                buffer[mBufferPos - 1] = (byte)'=';
                if (sizeMod == 1) buffer[mBufferPos - 2] = (byte)'=';
            }
            return buffer;
        }
    }
}
