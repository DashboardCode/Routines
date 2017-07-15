using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Vse.Routines.Json;

namespace Vse.Routines.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void JsonSerializeTestException()
        {
            var source1 = TestTool.CreateTestModel();
            var include1 = TestTool.CreateInclude();
            try
            {
                var formatter = JsonChainNodeTools.BuildFormatter(include1,
                    (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n, useToString: false)
                );
                var json = formatter(source1);
            }
            catch (NotSupportedException ex)
            {
                if (!ex.Message.Contains(@"/StorageModel/Key/Attributes"))
                {
                    throw;
                }
            }
        }

        private static bool GetStringArrayFormatter(StringBuilder sb, string[] t)
        {
            sb.Append("["); foreach (var i in t) sb.Append("\"").Append(i).Append("\"").Append(","); sb.Append("]"); return true;
        }

        private static bool GetStringIntFormatter(StringBuilder sb, int[] t)
        {
            sb.Append("["); foreach (var i in t) sb.Append(i).Append(","); sb.Append("]"); return true;
        }

        private static bool GetStringGuidFormatter(StringBuilder sb, IEnumerable<Guid> t)
        {
            sb.Append("["); foreach (var i in t) sb.Append("\"").Append(i).Append("\"").Append(","); sb.Append("]"); return true;
        }

        [TestMethod]
        public void JsonSerializeTestAddedFormatter()
        {
            var source1  = TestTool.CreateTestModel();
            var include1 = TestTool.CreateInclude();

            var formatter = JsonChainNodeTools.BuildFormatter(include1,
                n => JsonChainNodeTools.GetDefaultLeafSerializerSet(
                     n,
                     rulesDictionary: RulesDictionary
                        .CreateDefault()
                        .AddLeafTypeRule<string[]>(GetStringArrayFormatter)
                        .AddLeafTypeRule<int[]>(GetStringIntFormatter)
                        .AddLeafTypeRule<IEnumerable<Guid>>(GetStringGuidFormatter),
                     useToString: false),
                getInternalSerializerSet:null

            //f => 
            );
            var json = formatter(source1);
        }

        [TestMethod]
        public void JsonSerializeTest()
        {
            var source  = TestTool.CreateTestModel();
            var include = TestTool.CreateInclude();

            // TODO: 1) add nice error message "Node "" included as leaf but formatter of its type... is not setuped" 2) add string[] formatter
            var formatter = JsonChainNodeTools.BuildFormatter(include,
                    (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n)
            );
            var json = formatter(source);
            if (json!= "{\"StorageModel\":{\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":\"System.String[]\"},\"TableName\":\"TableName1\",\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":\"System.Int32[]\",\"ListTest\":\"System.Collections.Generic.List`1[System.Guid]\",\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable1\":null,\"IntNullable2\":555}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeNullTest()
        {
            var include = TestTool.CreateInclude();

            var formatter = JsonChainNodeTools.BuildFormatter(include
                    , (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n)
                    , rootHandleNull: false
            );
            var json = formatter(null);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeRootEmptyObjectLiteralOnTest()
        {
            Include<TestModel> include = (chain) => chain.Include(e=>e.IntNullable1).Include(e => e.IntNullable2);
            var source = new TestModel();
            var formatter = JsonChainNodeTools.BuildFormatter(include
                    , (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n, handleNullProperty:false)
                    , rootHandleEmptyObjectLiteral: true
            );
            var json = formatter(source);
            if (json != "{}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeRootEmptyObjectLiteralOffTest()
        {
            Include<TestModel> include = (chain) => chain.Include(e => e.IntNullable1).Include(e => e.IntNullable2);
            var source = new TestModel();
            var formatter = JsonChainNodeTools.BuildFormatter(include
                    , (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n, handleNullProperty: false)
                    , rootHandleEmptyObjectLiteral: false
            );
            var json = formatter(source);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeRootEmptyArrayLiteralOnTest()
        {
            Include<TestModel> include = (chain) => chain.Include(e => e.IntNullable1).Include(e => e.IntNullable2);
            var source = new TestModel[0];
            var formatter = JsonChainNodeTools.BuildEnumerableFormatter(include
                    , rootHandleEmptyArrayLiteral: true
            );
            var json = formatter(source);
            if (json != "[]")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeRootEmptyArrayLiteralOffTest()
        {
            Include<TestModel> include = (chain) => chain.Include(e => e.IntNullable1).Include(e => e.IntNullable2);
            var source = new TestModel[0];
            var formatter = JsonChainNodeTools.BuildEnumerableFormatter(include
                    , rootHandleEmptyArrayLiteral: false
            );
            var json = formatter(source);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStringNullTest()
        {
            var formatter = JsonChainNodeTools.BuildFormatter<string>(
                    include: null
                    , getLeafSerializerSet: (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n)
                    , rootHandleNull: false
            );
            var json = formatter(null);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonEnumerableSerializeTest()
        {
            var source1 = TestTool.CreateTestModel();
            var source2 = TestTool.CreateTestModel();
            var source = new[] { source1, source2 };
            var include1 = TestTool.CreateInclude();

            // TODO: 1) add nice error message "Node "" included as leaf but formatter of its type... is not setuped" 2) add string[] formatter
            var formatter = JsonChainNodeTools.BuildEnumerableFormatter(include1,
                    (n)   => JsonChainNodeTools.GetDefaultLeafSerializerSet(n),
                    (n,b) => JsonChainNodeTools.GetDefaultInternalSerializerSet(n,b, handleNullProperty:false, handleNullArrayProperty:false),
                    rootHandleNullArray:false
            );
            var json = formatter(source);
            if (json != "[{\"StorageModel\":{\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":\"System.String[]\"},\"TableName\":\"TableName1\",\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":\"System.Int32[]\",\"ListTest\":\"System.Collections.Generic.List`1[System.Guid]\",\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable1\":null,\"IntNullable2\":555},{\"StorageModel\":{\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":\"System.String[]\"},\"TableName\":\"TableName1\",\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":\"System.Int32[]\",\"ListTest\":\"System.Collections.Generic.List`1[System.Guid]\",\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable1\":null,\"IntNullable2\":555}]")
                throw new Exception(nameof(JsonEnumerableSerializeTest));
        }

        [TestMethod]
        public void JsonEnumerableSerializeNullTest()
        {
            var include1 = TestTool.CreateInclude();

            var formatter = JsonChainNodeTools.BuildEnumerableFormatter(include1,
                    (n) => JsonChainNodeTools.GetDefaultLeafSerializerSet(n),
                    (n, b) => JsonChainNodeTools.GetDefaultInternalSerializerSet(n, b, handleNullProperty: false, handleNullArrayProperty: false),
                    rootHandleNullArray: false
            );
            var json = formatter(null);
            if (json != "")
                throw new Exception(nameof(JsonEnumerableSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeDateTimeField()
        {
            var data = TestTool.CreateTestModel();
            Include<TestModel> include = chain => chain.Include(i => i.Message).ThenInclude(i => i.DateTimeMsg);

            var formatter = JsonChainNodeTools.BuildFormatter(include);
            var json = formatter(data);
            if (json != "{\"Message\":{\"DateTimeMsg\":\"9999-12-31T23:59:59.999\"}}")
                throw new Exception(nameof(JsonSerializeDateTimeField));
        }

        [TestMethod]
        public void JsonSerializeDateTimeCustomFormatField()
        {

            var data = TestTool.CreateTestModel();
            Include<TestModel> include = chain => chain.Include(i => i.Message).ThenInclude(i => i.DateTimeMsg);

            var formatter = JsonChainNodeTools.BuildFormatter(include, 
                n=> JsonChainNodeTools.GetDefaultLeafSerializerSet(n, 
                    rulesDictionary: 
                        RulesDictionary.CreateDefault(dateTimeFormat: "yyyy-MM-dd")
                    )
                );
            var json = formatter(data);
            if (json != "{\"Message\":{\"DateTimeMsg\":\"9999-12-31T23:59:59.999\"}}")
                throw new Exception(nameof(JsonSerializeDateTimeField));
        }
    }
}
