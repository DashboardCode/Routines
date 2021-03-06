﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.Test
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void JsonSerializeTestException()
        {
            var source1 = TestTool.CreateTestModel();
            var include1 = TestTool.CreateInclude();
            try
            {
                var formatter = JsonManager.ComposeFormatter(include1, useToString: false);
                var json = formatter(source1);
            }
            catch (NotConfiguredException ex)
            {
                if (!ex.Message.Contains(@"/StorageModel/Key/Attributes"))
                {
                    throw;
                }
            }
        }

        private static bool GetStringArrayFormatter(StringBuilder sb, string[] t)
        {
            sb.Append("["); foreach (var i in t) sb.Append("\"").Append(i).Append("\"").Append(",");
            if (t.Length > 0) sb.Length--;
            sb.Append("]");
            return true;
        }

        private static bool GetStringIntFormatter(StringBuilder sb, int[] t)
        {
            sb.Append("["); foreach (var i in t) sb.Append(i).Append(",");
            if (t.Length > 0) sb.Length--;
            sb.Append("]");
            return true;
        }

        private static bool GetSumFormatter(StringBuilder sb, int[] t)
        {
            var sum = 0;
            foreach (var i in t) sum += i;
            sb.Append(sum);
            return true;
        }

        private static bool GetStringGuidFormatter(StringBuilder sb, IEnumerable<Guid> t)
        {
            sb.Append("[");
            var e = t.GetEnumerator();
            bool moveNext = e.MoveNext();
            while (moveNext)
            {
                sb.Append("\"").Append(e.Current).Append("\"");
                moveNext = e.MoveNext();
                if (moveNext)
                    sb.Append(",");
            }
            sb.Append("]");
            return true;
        }

        [TestMethod]
        public void JsonSerializeRootEmptyArrayLiteralOnTest()
        {
            Include<TestModel> include = (chain) => chain.Include(e => e.IntNullable1).Include(e => e.IntNullable2);
#pragma warning disable CA1825 // Avoid zero-length array allocations.
            var source = new TestModel[0];
#pragma warning restore CA1825 // Avoid zero-length array allocations.
            var formatter = JsonManager.ComposeEnumerableFormatter(include, rootHandleEmptyLiteral: true);
            var json = formatter(source);
            if (json != "[]")
                throw new Exception(nameof(JsonSerializeTest));
        }

        struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        struct Point2
        {
            public string X { get; set; }
            public string Y { get; set; }
        }

        struct Point3
        {
            public int X { get; set; }
            public int Y { get; set; }
            public List<Point> Points { get; set; }
        }

        [TestMethod]
        public void JsonSerializeStructPoint()
        {
            Include<Point> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var source = new Point() {X=1, Y=1};
            var formatter = JsonManager.ComposeFormatter(include);
            var json = formatter(source);
            if (json != "{\"X\":1,\"Y\":1}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArray()
        {
            Include<Point> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var source = new Point() { X = 1, Y = 2 };
            var formatter = JsonManager.ComposeFormatter(include,objectAsArray:true);
            var json = formatter(source);
            if (json != "[1,2]")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArrayComplex()
        {
            Include<Point3> include = (chain) => chain.Include(e => e.X).Include(e => e.Y)
                .IncludeAll(e => e.Points).ThenIncluding(e=>e.X).ThenIncluding(e => e.Y);
            var source = new Point3() { X = 1, Y = 2, Points = new List<Point>() { new Point() { X = 3, Y = 4 }, new Point() { X = 5, Y = 6 } } };
            var formatter = JsonManager.ComposeFormatter(include, objectAsArray: true);
            var json = formatter(source);
            if (json != "[1,2,[[3,4],[5,6]]]")
                throw new Exception(nameof(JsonSerializeTest));
        }


        [TestMethod]
        public void JsonSerializeStructPointAsArrayAndProperty()
        {
            Include<Point> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var x = 3;
            var formatter = JsonManager.ComposeFormatter(
                include, objectAsArray: true, rootAsProperty:"data", rootPropertyAppender: j=>j.AddNumberProperty("extra",x).AddStringProperty("extra2","\""));

            var source1 = new Point() { X = 1, Y = 2 };
            var json1 = formatter(source1);
            if (json1 != "{\"data\":[1,2],\"extra\":3,\"extra2\":\"\\\"\"}")
                throw new Exception(nameof(JsonSerializeTest));

            x = -3;
            var source2 = new Point() { X = 2, Y = 1 };
            var json2 = formatter(source2);
            if (json2 != "{\"data\":[2,1],\"extra\":-3,\"extra2\":\"\\\"\"}")
                throw new Exception(nameof(JsonSerializeTest));
        }
        
        [TestMethod]
        public void JsonSerializeStructPointAsArrayAndPropertyCached()
        {
            var source1 = new[] { new Point() { X = 1, Y = 2 }, new Point() { X = 3, Y = 4 } };
            var json1 = JsonSerializeStructPointAsArrayAndPropertyCachedHelper1(source1, 1);
            if (json1 != "{\"data\":[[2],[4]],\"added\":1}")
                throw new Exception(nameof(JsonSerializeTest));

            var source2 = new[] { new Point() { X = 4, Y = 3 }, new Point() { X = 2, Y = 1 } };
            var json2 = JsonSerializeStructPointAsArrayAndPropertyCachedHelper1(source2, 2);
            if (json2 != "{\"data\":[[5],[3]],\"added\":1}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        readonly static CachedFormatter cachedFormatter1 = new CachedFormatter();
        private string JsonSerializeStructPointAsArrayAndPropertyCachedHelper1(Point[] source, int x)
        {
            
            var json = source.ToJsonAll(
                cachedFormatter1,
                chain => chain.Include(e => e.X + x, "IX"),
                objectAsArray: true, 
                rootAsProperty: "data",
                rootPropertyAppender: j => j.AddNumberProperty("added", x));
            return json;
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArrayAndPropertyCached2()
        {
            var source1 = new[] { new Point() { X = 1, Y = 2 }, new Point() { X = 3, Y = 4 } };
            var json1 = JsonSerializeStructPointAsArrayAndPropertyCachedHelper2(source1, 1);
            if (json1 != "{\"data\":[[2],[4]],\"added\":1}")
                throw new Exception(nameof(JsonSerializeTest));

            var source2 = new[] { new Point() { X = 1, Y = 2 }, new Point() { X = 3, Y = 4 } };
            var json2 = JsonSerializeStructPointAsArrayAndPropertyCachedHelper2(source2, 2);
            if (json2 != "{\"data\":[[2],[4]],\"added\":2}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        readonly static CachedFormatter cachedFormatter2 = new CachedFormatter();
        private string JsonSerializeStructPointAsArrayAndPropertyCachedHelper2(Point[] source, int closure)
        {
            var json = source.ToJsonAll(
                closure,
                cachedFormatter2,
                chain => chain.Include(e => e.X + closure, "IX"),
                objectAsArray: true,
                rootAsProperty: "data",
                rootPropertyAppender: (j, c) => j.AddNumberProperty("added", c));
            return json;
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArray2()
        {
            Include<Point2> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var source = new Point2() { X = null, Y = null };
            var formatter = JsonManager.ComposeFormatter(include, objectAsArray: true,handleNullProperty:false );
            var json = formatter(source);
            if (json != "[]")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArray2AsProperty()
        {
            Include<Point2> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var source = new Point2() { X = null, Y = null };
            var formatter = JsonManager.ComposeFormatter(include, objectAsArray: true, rootAsProperty: "data", handleNullProperty: false, rootHandleEmptyLiteral: false);
            var json = formatter(source);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArray2b()
        {
            Include<Point2> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var source = new Point2() { X = null, Y = null };
            var formatter = JsonManager.ComposeFormatter(include, objectAsArray: true, handleNullProperty: false, rootHandleEmptyLiteral: false );
            var json = formatter(source);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStructPointAsArray3()
        {
            Include<Point2> include = (chain) => chain.Include(e => e.X).Include(e => e.Y);
            var source = new Point2() { X = null, Y = "" };
            var formatter = JsonManager.ComposeFormatter(include, objectAsArray: true, handleNullProperty: false);
            var json = formatter(source);
            if (json != "[,\"\"]")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStructPointCreateFormatter()
        {
            var source = new Point() { X = 1, Y = 1 };
            var formatter = JsonManager.ComposeFormatter<Point>();
            var json = formatter(source);
            if (json != "{\"X\":1,\"Y\":1}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeTestAddedFormatterAndCustomInclude()
        {
            var source  = TestTool.CreateTestModel();
            var include = TestTool.CreateInclude();

            //Include<TestModel, int[]> pathInclude = (path) => path.ThenInclude(e=>e.)

            var formatter = JsonManager.ComposeFormatter(
                include,
                rules => rules
                    .AddRule<string[]>(GetStringArrayFormatter)
                    .AddRule<int[]>((sb, l) => GetStringIntFormatter(sb, l))
                    .AddRule<IEnumerable<Guid>>(GetStringGuidFormatter)
                    .SubTree(
                        chain  => chain.Include(e => e.Test),
                        subRules => subRules.AddRule<int[]>(serializer: GetSumFormatter, propertySerializationName: "Sum")
                    ),
                useToString: false,
                dateTimeFormat: null, 
                floatingPointFormat: null);

            var json = formatter(source);
            if (json != "{\"StorageModel\":{\"TableName\":\"TableName1\",\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":[\"FieldA1\",\"FieldA2\"]},\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Sum\":6,\"ListTest\":[\"360bc50a-4d9f-4703-bbea-58f67a6ff475\",\"f2ecf4d8-f4a6-446c-a363-cc79b02decdd\"],\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable1\":null,\"IntNullable2\":555}")
                throw new Exception(nameof(JsonSerializeTestAddedFormatter));
        }

        [TestMethod]
        public void JsonSerializeTestAddedFormatter()
        {
            var source = TestTool.CreateTestModel();
            var include = TestTool.CreateInclude();

            var formatter = JsonManager.ComposeFormatter(
                include,
                rules => rules
                        .AddRule<string[]>(GetStringArrayFormatter)
                        .AddRule<int[]>((sb, l) => GetStringIntFormatter(sb, l))
                        .AddRule<IEnumerable<Guid>>(GetStringGuidFormatter),
                useToString: false
                );

            var json = formatter(source);
            if (json != "{\"StorageModel\":{\"TableName\":\"TableName1\",\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":[\"FieldA1\",\"FieldA2\"]},\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":[1,2,3],\"ListTest\":[\"360bc50a-4d9f-4703-bbea-58f67a6ff475\",\"f2ecf4d8-f4a6-446c-a363-cc79b02decdd\"],\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable1\":null,\"IntNullable2\":555}")
                throw new Exception(nameof(JsonSerializeTestAddedFormatter));
        }


        [TestMethod]
        public void JsonSerializeTest()
        {
            var source  = TestTool.CreateTestModel();
            var include = TestTool.CreateInclude();

            // TODO: 1) add nice error message "Node "" included as leaf but formatter of its type... is not setuped" 2) add string[] formatter
            var formatter = JsonManager.ComposeFormatter(include, useToString: true);
            var json = formatter(source);
            if (json!= "{\"StorageModel\":{\"TableName\":\"TableName1\",\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":\"System.String[]\"},\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":\"System.Int32[]\",\"ListTest\":\"System.Collections.Generic.List`1[System.Guid]\",\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable1\":null,\"IntNullable2\":555}")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeNullTest()
        {
            var include = TestTool.CreateInclude();

            var formatter = JsonManager.ComposeFormatter(include
                , rootHandleNull: false, useToString: true
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
            var formatter = JsonManager.ComposeFormatter(
                    include
                    , handleNullProperty: false
                    , rootHandleEmptyLiteral: true
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
            var formatter = JsonManager.ComposeFormatter(include
                    , handleNullProperty: false
                    , rootHandleEmptyLiteral: false
            );
            var json = formatter(source);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeRootEmptyArrayLiteralOffTest()
        {
            Include<TestModel> include = (chain) => chain.Include(e => e.IntNullable1).Include(e => e.IntNullable2);
#pragma warning disable CA1825 // Avoid zero-length array allocations.
            var source = new TestModel[0];
#pragma warning restore CA1825 // Avoid zero-length array allocations.
            var formatter = JsonManager.ComposeEnumerableFormatter(include, rootHandleEmptyLiteral: false);
            var json = formatter(source);
            if (json != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonSerializeStringNullTest()
        {
            var formatter1 = JsonManager.ComposeFormatter<string>(rootHandleNull: true);
            var json1 = formatter1(null);
            if (json1 != "null")
                throw new Exception(nameof(JsonSerializeTest));
            var formatter2 = JsonManager.ComposeFormatter<string>(rootHandleNull: false);
            var json2 = formatter2(null);
            if (json2 != "")
                throw new Exception(nameof(JsonSerializeTest));
        }

        [TestMethod]
        public void JsonEnumerableSerializeTest()
        {
            var source = new[] { TestTool.CreateTestModel(), TestTool.CreateTestModel() };
            var include = TestTool.CreateInclude();

            // TODO: 1) add nice error message "Node "" included as leaf but formatter of its type... is not setuped" 2) add string[] formatter
            var formatter = JsonManager.ComposeEnumerableFormatter(include
                    , handleNullProperty: false
                    , handleNullArrayProperty: false
                    , rootHandleNull:false
                    , useToString: true
            );
            var json = formatter(source);
            if (json != "[{\"StorageModel\":{\"TableName\":\"TableName1\",\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":\"System.String[]\"},\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":\"System.Int32[]\",\"ListTest\":\"System.Collections.Generic.List`1[System.Guid]\",\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable2\":555},{\"StorageModel\":{\"TableName\":\"TableName1\",\"Entity\":{\"Name\":\"EntityName1\",\"Namespace\":\"EntityNamespace1\"},\"Key\":{\"Attributes\":\"System.String[]\"},\"Uniques\":[{\"IndexName\":\"IndexName1\",\"Fields\":[\"FieldU1\"]},{\"IndexName\":\"IndexName2\",\"Fields\":[\"FieldU2\"]}]},\"Test\":\"System.Int32[]\",\"ListTest\":\"System.Collections.Generic.List`1[System.Guid]\",\"Message\":{\"TextMsg\":\"Initial\",\"DateTimeMsg\":\"9999-12-31T23:59:59.999\",\"IntNullableMsg\":7},\"IntNullable2\":555}]")
                throw new Exception(nameof(JsonEnumerableSerializeTest));
        }

        [TestMethod]
        public void JsonEnumerableSerializeNullTest()
        {
            var include = TestTool.CreateInclude();

            var formatter = JsonManager.ComposeEnumerableFormatter(include
                    , rootHandleNull: false
                    , handleNullProperty: false
                    , handleNullArrayProperty: false
                    , useToString: true
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

            var formatter = JsonManager.ComposeFormatter(include);
            var json = formatter(data);
            if (json != "{\"Message\":{\"DateTimeMsg\":\"9999-12-31T23:59:59.999\"}}")
                throw new Exception(nameof(JsonSerializeDateTimeField));
        }

        [TestMethod]
        public void JsonSerializeDateTimeCustomFormatField()
        {
            var data = TestTool.CreateTestModel();
            Include<TestModel> include = chain => chain.Include(i => i.Message).ThenInclude(i => i.DateTimeMsg);

            var formatter = JsonManager.ComposeFormatter(include, dateTimeFormat: "yyyy-MM-dd");
            var json = formatter(data);
            if (json != "{\"Message\":{\"DateTimeMsg\":\"9999-12-31\"}}")
                throw new Exception(nameof(JsonSerializeDateTimeField));
        }

        [TestMethod]
        public void JsonSerializeFloatingPointCustomFormatField()
        {
            {
                var formatter1 = JsonManager.ComposeFormatter<float>();
                var json1 = formatter1((float)1 / 3);
                if (json1 != "0.3333333")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "1");

                var formatter1e = JsonManager.ComposeEnumerableFormatter<float>();
                var json1e = formatter1e(new float[] { (float)1 / 3, (float)Math.Sqrt(7) });
                if (json1e != "[0.3333333,2.645751]")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "1e");

                var formatter2 = JsonManager.ComposeFormatter<float>(floatingPointFormat: "N4");
                var json2 = formatter2((float)1 / 3);
                if (json2 != "0.3333")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "2");

                var formatter2e = JsonManager.ComposeEnumerableFormatter<float>(floatingPointFormat: "N4");
                var json2e = formatter2e(new float[] { (float)1 / 3, (float)Math.Sqrt(7) });
                if (json2e != "[0.3333,2.6458]")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "3");
            }

            {
                var formatter1 = JsonManager.ComposeFormatter<double>();
                var json1 = formatter1((double)1 / 3);
                if (json1 != "0.333333333333333")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "1D");

                var formatter1e = JsonManager.ComposeEnumerableFormatter<double>();
                var json1e = formatter1e(new double[] { (double)1 / 3, Math.Sqrt(7) });
                if (json1e != "[0.333333333333333,2.64575131106459]")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "1De");

                var formatter2 = JsonManager.ComposeFormatter<double>(floatingPointFormat: "N4");
                var json2 = formatter2((double)1 / 3);
                if (json2 != "0.3333")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "2D");

                var formatter2e = JsonManager.ComposeEnumerableFormatter<double>(floatingPointFormat: "N4");
                var json2e = formatter2e(new double[] { (double)1 / 3, Math.Sqrt(7) });
                if (json2e != "[0.3333,2.6458]")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "3D");
            }
            {
                var formatter1 = JsonManager.ComposeFormatter<float?>();
                var json1 = formatter1((float)1 / 3);
                if (json1 != "0.3333333")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "1N");

                var json2 = formatter1(null);
                if (json2 != "null")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "2N");

                var formatter2e = JsonManager.ComposeEnumerableFormatter<float?>();
                var json2e = formatter2e(new float?[] { (float)1 / 3, null });
                if (json2e != "[0.3333333,null]")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "3Ne");

                var json3e = formatter2e(null);
                if (json3e != "null")
                    throw new Exception(nameof(JsonSerializeDateTimeField) + "4Ne");
            }
        }

        [TestMethod]
        public void JsonSerializeNodeSpecailFormat()
        {

            var data = TestTool.CreateTestModel();
            Include<TestModel> include = chain => chain.Include(i => i.Message).ThenInclude(i => i.DateTimeMsg);

            var formatter = JsonManager.ComposeFormatter(include,
                dateTimeFormat: "yyyy-MM-dd"
                );
            var json = formatter(data);
            if (json != "{\"Message\":{\"DateTimeMsg\":\"9999-12-31\"}}")
                throw new Exception(nameof(JsonSerializeDateTimeField));
        }
    }
}
