using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Vse.Routines;
using Vse.Routines.Json;

namespace JsonNet.Test
{
    [TestClass]
    public class JsonSerializationTest
    {
        private static TestClass CreateTestClass() {
            var testClass = new TestClass()
            {
                Ints = new List<int> { 1, 2, 3 },
                NInts = new List<int?> { null, 1, 2, null, 3, null },
                BoolField = true,
                NBoolField1 = true,
                NBoolField2 = null,
                TextField1 = "TextField1TextField1TextField1",
                TextField2 = null,
                Float0 = 0,
                Float1 = ((float)1) / 3,
                TestClass1 = new TestClass()
                {
                    TextField1 = "TextField2TextField2TextField2",
                    TestClass1 = new TestClass() { TextField1 = "333333333" },
                    BoolField = true
                },
                TestStruct = new TestStruct()
                {
                    TextField1 = "TestStruct.TextField1",
                    Byte1 = 4,
                    Byte2 = null
                },
                NTestStruct1 = new TestStruct()
                {
                    TextField1 = "NTestStruct1.TextField1",
                    Byte1 = Byte.MaxValue,
                    Byte2 = null
                },
                NTestStruct2 = null,
                Number = 8,
                NNumber1 = -10,
                NNumber2 = null,
                ListItems = new List<ListItem> {
                    new ListItem {DateTime=DateTime.Now,      RowData=new byte[]{0,1}},
                    new ListItem {DateTime=DateTime.MinValue, RowData=new byte[]{2,3}},
                    new ListItem {DateTime=DateTime.Now,      RowData=new byte[]{}},
                    new ListItem {DateTime=DateTime.Now,      RowData=null}
                },
                RowData = new byte[] { 1, 0, 2, 0, 3, 4, 5 },
                TestRef1 = new TestRef() {Msg="abc"},
                TestRef2 = null,
            };
            return testClass;
        }

        Include<TestClass> CreateIncludes()
        {
            Include<TestClass> include = (i) =>
                                       i.Include(e => e.RowData)
                                       .IncludeAll(e => e.Ints)
                                       .IncludeAll(e => e.NInts)
                                       .Include(e => e.RowData)
                                       .IncludeAll(e => e.ListItems)
                                            .ThenInclude(e => e.RowData)
                                        .IncludeAll(e => e.ListItems)
                                            .ThenInclude(e => e.DateTime)
                                        .Include(e => e.TestStruct)
                                            .ThenInclude(e => e.TextField1)
                                        .Include(e => e.TestStruct)
                                            .ThenInclude(e => e.Byte1)
                                        .Include(e => e.TestStruct)
                                            .ThenInclude(e => e.Byte2)
                                        .Include(e => e.TestClass2)
                                        .Include(e => e.TestClass1)
                                            .ThenInclude(e => e.TextField1)
                                        .Include(e => e.TestClass1)
                                            .ThenInclude(e => e.BoolField)
                                        .Include(e => e.BoolField)
                                        .Include(e => e.NBoolField1)
                                        .Include(e => e.NBoolField2)
                                        .Include(e => e.Number)
                                        .Include(e => e.NNumber1)
                                        .Include(e => e.NNumber2)
                                        .Include(e => e.Float0)
                                        .Include(e => e.Float1)
                                        .Include(e => e.TextField1)
                                        .Include(e => e.TextField2)
                                        .Include(e => e.TestRef1)
                                            .ThenInclude(e => e.Msg)
                                        .Include(e => e.TestRef2)
                                            .ThenInclude(e => e.Msg);
            return include;
        }

        [TestMethod]
        public void JsonBuildSerializer()
        {
            var testClass = CreateTestClass();
            var stringBuilder = new StringBuilder();
            var include = CreateIncludes();

            var formatter = NExpJsonSerializerTools.BuildFormatter(include);
            var json = formatter(testClass);
        }


        [TestMethod]
        public void JsonBuildEnumerableSerializer()
        {
            List<TestClass> list = new List<TestClass>();
            var testClass = CreateTestClass();
            list.Add(testClass);
            list.Add(testClass);
            list.Add(testClass);
            var include = CreateIncludes();

            var formatter = NExpJsonSerializerTools.BuildEnumerableFormatter(include);
            var json = formatter(list);
        }

        [TestMethod]
        public void JsonBuildLeafSerializer()
        {
            string testValue = null;
            var stringBuilder = new StringBuilder();
            var formatter = NExpJsonSerializerTools.BuildFormatter<string>();
            var json = formatter(testValue);
        }

        [TestMethod]
        public void JsonBuildEnumerableLeafSerializer()
        {
            var testValue = new List<DateTime> { DateTime.MinValue, DateTime.Now };
            var stringBuilder = new StringBuilder();
            var formatter = NExpJsonSerializerTools.BuildEnumerableFormatter<DateTime>();
            var json = formatter(testValue);
        }
    }
}
