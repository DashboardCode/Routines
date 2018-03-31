using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;

namespace Benchmark
{
    [Config(typeof(MyManualConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkStructOrClass
    {
        static TestStruct testStruct = new TestStruct();
        static TestClass testClass = new TestClass();
        static TestStruct8 testStruct8 = new TestStruct8();
        static TestClass8 testClass8 = new TestClass8();
        [Benchmark]
        public void TestStructReturn()
        {
            testStruct.TestMethod();
        }

        [Benchmark]
        public void TestClassReturn()
        {
            testClass.TestMethod();
        }


        [Benchmark]
        public void TestStructReturn8()
        {
            testStruct8.TestMethod();
        }

        [Benchmark]
        public void TestClassReturn8()
        {
            testClass8.TestMethod();
        }

        public class TestStruct
        {
            public int Number = 5;
            public struct StructType<T>
            {
                public T Instance;
                public List<string> List;
            }

            public int TestMethod()
            {
                var s = Method1(1);
                return s.Instance;
            }

            private StructType<int> Method1(int i)
            {
                return Method2(++i);
            }

            private StructType<int> Method2(int i)
            {
                return Method3(++i);
            }

            private StructType<int> Method3(int i)
            {
                return Method4(++i);
            }

            private StructType<int> Method4(int i)
            {
                var x = new StructType<int>();
                x.List = new List<string>();
                x.Instance = ++i;
                return x;
            }
        }

        public class TestClass
        {
            public int Number = 5;
            public class ClassType<T>
            {
                public T Instance;
                public List<string> List;
            }

            public int TestMethod()
            {
                var s = Method1(1);
                return s.Instance;
            }

            private ClassType<int> Method1(int i)
            {
                return Method2(++i);
            }

            private ClassType<int> Method2(int i)
            {
                return Method3(++i);
            }

            private ClassType<int> Method3(int i)
            {
                return Method4(++i);
            }

            private ClassType<int> Method4(int i)
            {
                var x = new ClassType<int>();
                x.List = new List<string>();
                x.Instance = ++i;
                return x;
            }
        }

        public class TestStruct8
        {
            public int Number = 5;
            public struct StructType<T>
            {
                public T Instance1;
                public T Instance2;
                public T Instance3;
                public T Instance4;
                public T Instance5;
                public T Instance6;
                public T Instance7;
                public List<string> List;
            }

            public int TestMethod()
            {
                var s = Method1(1);
                return s.Instance1;
            }

            private StructType<int> Method1(int i)
            {
                return Method2(++i);
            }

            private StructType<int> Method2(int i)
            {
                return Method3(++i);
            }

            private StructType<int> Method3(int i)
            {
                return Method4(++i);
            }

            private StructType<int> Method4(int i)
            {
                var x = new StructType<int>();
                x.List = new List<string>();
                x.Instance1 = ++i;
                return x;
            }
        }

        public class TestClass8
        {
            public int Number = 5;
            public class ClassType<T>
            {
                public T Instance1;
                public T Instance2;
                public T Instance3;
                public T Instance4;
                public T Instance5;
                public T Instance6;
                public T Instance7;
                public List<string> List;
            }

            public int TestMethod()
            {
                var s = Method1(1);
                return s.Instance1;
            }

            private ClassType<int> Method1(int i)
            {
                return Method2(++i);
            }

            private ClassType<int> Method2(int i)
            {
                return Method3(++i);
            }

            private ClassType<int> Method3(int i)
            {
                return Method4(++i);
            }

            private ClassType<int> Method4(int i)
            {
                var x = new ClassType<int>();
                x.List = new List<string>();
                x.Instance1 = ++i;
                return x;
            }
        }
    }
}
