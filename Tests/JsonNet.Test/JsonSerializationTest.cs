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
                Ints  = new List<int> { 1, 2, 3 },
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
                    Byte2 = null,
                    Decimal1 = 100,
                    Decimal2 = null
                },
                NTestStruct1 = new TestStruct()
                {
                    TextField1 = "NTestStruct1.TextField1",
                    Byte1 = Byte.MaxValue,
                    Byte2 = null,
                    Decimal1 = 100,
                    Decimal2 = -100
                },
                NTestStruct2 = null,
                Number = 8,
                NNumber1 = -10,
                NNumber2 = null,
                ListItems = new List<ListItem> {
                    new ListItem {DateTime=new DateTime(36323577782833637),  RowData=new byte[]{0,1}},
                    new ListItem {DateTime=DateTime.MinValue,                RowData=new byte[]{2,3}},
                    new ListItem {DateTime=new DateTime(636323577782833637), RowData=new byte[]{}},
                    new ListItem {DateTime=new DateTime(6323577782833637),   RowData=null}
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
                                            .ThenInclude(e => e.Decimal1)
                                        .Include(e => e.TestStruct)
                                            .ThenInclude(e => e.Decimal2)
                                        //.Include(e => e.NTestStruct1)
                                        //    .ThenInclude(e => e.Value.Decimal1)
                                        //.IncludeNullable(e => e.NTestStruct1)
                                        //    .ThenInclude(e => e.Decimal1)
                                        //.IncludeNullable(e => e.NTestStruct1)
                                        //    .ThenInclude(e => e.Decimal2)
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

            var formatter = TrainJsonTools.BuildFormatter/*<TestClass>*/(
                include,
                (n, b) => TrainJsonTools.GetDefaultSerializerSet(n, b)
            );
            
            var json = formatter(testClass);
            if (json != "{\"RowData\":\"AQACAAMEBQ==\",\"Ints\":[1,2,3],\"NInts\":[null,1,2,null,3,null],\"ListItems\":[{\"RowData\":\"AAE=\",\"DateTime\":\"0116-02-09T04:16:18.283\"},{\"RowData\":\"AgM=\",\"DateTime\":\"0001-01-01T00:00:00.000\"},{\"RowData\":\"\",\"DateTime\":\"2017-06-06T14:56:18.283\"},{\"RowData\":null,\"DateTime\":\"0021-01-14T22:56:18.283\"}],\"TestStruct\":{\"TextField1\":\"TestStruct.TextField1\",\"Decimal1\":100,\"Decimal2\":null,\"Byte1\":4,\"Byte2\":null},\"TestClass2\":null,\"TestClass1\":{\"TextField1\":\"TextField2TextField2TextField2\",\"BoolField\":true},\"BoolField\":true,\"NBoolField1\":true,\"NBoolField2\":null,\"Number\":8,\"NNumber1\":-10,\"NNumber2\":null,\"Float0\":0,\"Float1\":0.3333333,\"TextField1\":\"TextField1TextField1TextField1\",\"TextField2\":null,\"TestRef1\":{\"Msg\":\"abc\"},\"TestRef2\":null}")
                throw new AssertFailedException("json not correct");
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
            var formatter = TrainJsonTools.BuildEnumerableFormatter(include,
                    (n, b) => TrainJsonTools.GetDefaultSerializerSet(n, b)
            );
            var json = formatter(list);
            if(json != "[{\"RowData\":\"AQACAAMEBQ==\",\"Ints\":[1,2,3],\"NInts\":[null,1,2,null,3,null],\"ListItems\":[{\"RowData\":\"AAE=\",\"DateTime\":\"0116-02-09T04:16:18.283\"},{\"RowData\":\"AgM=\",\"DateTime\":\"0001-01-01T00:00:00.000\"},{\"RowData\":\"\",\"DateTime\":\"2017-06-06T14:56:18.283\"},{\"RowData\":null,\"DateTime\":\"0021-01-14T22:56:18.283\"}],\"TestStruct\":{\"TextField1\":\"TestStruct.TextField1\",\"Decimal1\":100,\"Decimal2\":null,\"Byte1\":4,\"Byte2\":null},\"TestClass2\":null,\"TestClass1\":{\"TextField1\":\"TextField2TextField2TextField2\",\"BoolField\":true},\"BoolField\":true,\"NBoolField1\":true,\"NBoolField2\":null,\"Number\":8,\"NNumber1\":-10,\"NNumber2\":null,\"Float0\":0,\"Float1\":0.3333333,\"TextField1\":\"TextField1TextField1TextField1\",\"TextField2\":null,\"TestRef1\":{\"Msg\":\"abc\"},\"TestRef2\":null},{\"RowData\":\"AQACAAMEBQ==\",\"Ints\":[1,2,3],\"NInts\":[null,1,2,null,3,null],\"ListItems\":[{\"RowData\":\"AAE=\",\"DateTime\":\"0116-02-09T04:16:18.283\"},{\"RowData\":\"AgM=\",\"DateTime\":\"0001-01-01T00:00:00.000\"},{\"RowData\":\"\",\"DateTime\":\"2017-06-06T14:56:18.283\"},{\"RowData\":null,\"DateTime\":\"0021-01-14T22:56:18.283\"}],\"TestStruct\":{\"TextField1\":\"TestStruct.TextField1\",\"Decimal1\":100,\"Decimal2\":null,\"Byte1\":4,\"Byte2\":null},\"TestClass2\":null,\"TestClass1\":{\"TextField1\":\"TextField2TextField2TextField2\",\"BoolField\":true},\"BoolField\":true,\"NBoolField1\":true,\"NBoolField2\":null,\"Number\":8,\"NNumber1\":-10,\"NNumber2\":null,\"Float0\":0,\"Float1\":0.3333333,\"TextField1\":\"TextField1TextField1TextField1\",\"TextField2\":null,\"TestRef1\":{\"Msg\":\"abc\"},\"TestRef2\":null},{\"RowData\":\"AQACAAMEBQ==\",\"Ints\":[1,2,3],\"NInts\":[null,1,2,null,3,null],\"ListItems\":[{\"RowData\":\"AAE=\",\"DateTime\":\"0116-02-09T04:16:18.283\"},{\"RowData\":\"AgM=\",\"DateTime\":\"0001-01-01T00:00:00.000\"},{\"RowData\":\"\",\"DateTime\":\"2017-06-06T14:56:18.283\"},{\"RowData\":null,\"DateTime\":\"0021-01-14T22:56:18.283\"}],\"TestStruct\":{\"TextField1\":\"TestStruct.TextField1\",\"Decimal1\":100,\"Decimal2\":null,\"Byte1\":4,\"Byte2\":null},\"TestClass2\":null,\"TestClass1\":{\"TextField1\":\"TextField2TextField2TextField2\",\"BoolField\":true},\"BoolField\":true,\"NBoolField1\":true,\"NBoolField2\":null,\"Number\":8,\"NNumber1\":-10,\"NNumber2\":null,\"Float0\":0,\"Float1\":0.3333333,\"TextField1\":\"TextField1TextField1TextField1\",\"TextField2\":null,\"TestRef1\":{\"Msg\":\"abc\"},\"TestRef2\":null}]")
                throw new AssertFailedException("json not correct");
        }

        [TestMethod]
        public void JsonBuildLeafSerializer()
        {
            string testValue = null;
            var stringBuilder = new StringBuilder();
            var formatter = TrainJsonTools.BuildFormatter<string>();
            var json = formatter(testValue);
            if (json != "null")
                throw new AssertFailedException("json not null");
        }

        [TestMethod]
        public void JsonBuildEnumerableLeafSerializer()
        {
            var testValue = new List<DateTime> { DateTime.MinValue, new DateTime(36323577782833637) };
            var stringBuilder = new StringBuilder();
            var formatter = TrainJsonTools.BuildEnumerableFormatter<DateTime>();
            var json = formatter(testValue);
            if (json!="[\"0001-01-01T00:00:00.000\",\"0116-02-09T04:16:18.283\"]")
                throw new AssertFailedException("json not correct");
        }

        #region NInt
        [TestMethod]
        public void JsonBuildLeafNintNullSerializer()
        {
            int? x = null;
            var stringBuilder = new StringBuilder();
            var formatter = TrainJsonTools.BuildFormatter<int?>();
            var json = formatter(x);
            if (json != "null")
                throw new AssertFailedException("json not null");
        }

        [TestMethod]
        public void JsonBuildLeafNintSerializer()
        {
            int? x = 10;
            var stringBuilder = new StringBuilder();
            var formatter = TrainJsonTools.BuildFormatter<int?>();
            var json = formatter(x);
            if (json != "10")
                throw new AssertFailedException("json not 10");
        }

        [TestMethod]
        public void JsonBuildNIntEnumerableSerializer()
        {
            var list = new List<int?>();
            var testClass = CreateTestClass();
            list.Add(0);
            list.Add(null);
            list.Add(1);

            var formatter = TrainJsonTools.BuildEnumerableFormatter<int?>();
            var json = formatter(list);
            if (json!="[0,null,1]")
                throw new AssertFailedException("json not [0,null,1]");
        }

        [TestMethod]
        public void JsonBuildNIntNullEnumerableSerializer()
        {
            var formatter = TrainJsonTools.BuildEnumerableFormatter<int?>();
            var json = formatter(null);
            if (json != "null")
                throw new AssertFailedException("json not null");
        }
        #endregion

        [TestMethod]
        public void JsonBuildEnumerableLeafNullSerializer()
        {
            var stringBuilder = new StringBuilder();
            var formatter = TrainJsonTools.BuildEnumerableFormatter<DateTime>();
            var json = formatter(null);
            if (json != "null")
                throw new AssertFailedException("json not null");
        }
    }
}
