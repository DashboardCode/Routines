using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
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
            var t = new Vse.Routines.Json.Test()
            {
                TextField1 = "TextField1TextField1TextField1",
                TestField = new Vse.Routines.Json.Test()
                {
                    TextField1 = "TextField2TextField2TextField2",
                    TestField = new Vse.Routines.Json.Test() { TextField1 = "333333333"},
                    BoolField1 = true
                },
                BoolField1 = true,
                ListItems = new List<ListItem> {
                    new ListItem {DateTime=DateTime.Now,      RowData=new byte[]{0,1}},
                    new ListItem {DateTime=DateTime.MinValue, RowData=new byte[]{2,3}},
                    new ListItem {DateTime=DateTime.Now,      RowData=new byte[]{}},
                    new ListItem {DateTime=DateTime.Now,      RowData=null}
                }
            };
            var stringBuilder = new StringBuilder();
            //var serializer = NExpJsonSerializerTools.BuildSerializerX(/*sb, t*/);
            var serializer = NExpJsonSerializerTools.BuildSerializerReflection<Vse.Routines.Json.Test>(/*sb, t*/);
            serializer(stringBuilder, t);
            var json = stringBuilder.ToString();
        }

    }
}
