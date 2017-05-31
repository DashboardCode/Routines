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
        private class NullTestClass
        {
            public int? Int1NullableValue { get; set; } = null;
            public string StringValue { get; set; } = null;
        }

        [TestMethod]
        public void AAATest()
        {
            var t = new TestClass()
            {
                Ints = new List<int> { 1, 2, 3 },
                NInts = new List<int?> { null, 1, 2, null, 3,null },
                BoolField = true,
                NBoolField1 = true,
                NBoolField2 = null,
                TextField1 = "TextField1TextField1TextField1",
                TextField2 = null,
                Float0 = 0,
                Float1 = ((float)1)/3,
                TestClass1 = new TestClass()
                {
                    TextField1  = "TextField2TextField2TextField2",
                    TestClass1  = new TestClass() { TextField1 = "333333333"},
                    BoolField   = true
                },
                TestStruct = new TestStruct()
                {
                    TextField1= "TestStruct.TextField1",
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
                RowData = new byte[] { 1,0,2,0,3,4,5 }
            };
            var stringBuilder = new StringBuilder();

            var parser = new SerializerNExpParser<TestClass>();
            var includable = new Includable<TestClass>(parser);
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
                                        ;
            include.Invoke(includable);
            var serializerNode = parser.Root;

            Include<TestClass> optionMembers1 = (i) => i.Include(e => e.TextField1);
            
            var serializer = NExpJsonSerializerTools.BuildSerializer<TestClass>(serializerNode);
            
            //serializerNode.AppendLeafs();

            //var serializer = NExpJsonSerializerTools.BuildSerializerX(/*sb, t*/);
            //var serializer = NExpJsonSerializerTools.BuildSerializer3<Vse.Routines.Json.Test>(/*sb, t*/);
            serializer(stringBuilder, t);
            var json = stringBuilder.ToString();
        }
    }
}
