using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vse.Routines.Json;

namespace Vse.Routines.Test
{
    [TestClass]
    public class JsonValidationTest
    {
        private class NullTestClass
        {
            public int? Int1NullableValue { get; set; } = null;
            public string StringValue { get; set; } = null;
        }

        //[TestMethod]
        //public void JsonNull1Test()
        //{
        //    var t = default(NullTestClass);
        //    Include<NullTestClass> includes = null;
        //    var json1 = includes.SerializeJson(t, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text1 = JsonConvert.SerializeObject(t, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Include});
        //    if (json1 != text1)
        //        throw new Exception("test 1");

        //    var json2 = includes.SerializeJson(t, new NExpJsonSerializerSettings() { NullValueHandling = false });
        //    string text2 = JsonConvert.SerializeObject(t, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});
        //    if (json2 != text2)
        //        throw new Exception("test 2");

        //    t = new NullTestClass();
        //    var json3 = includes.SerializeJson(t, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text3 = JsonConvert.SerializeObject(t, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json3 != text3)
        //        throw new Exception("test 3");
        //}

        private class NullTestClass2
        {
            public NullTestClass NullTestClassParent { get; set; } = null;
            public int? Int1NullableValue { get; set; } = 5;
            public string StringValue { get; set; } = null;
        }

        //[TestMethod]
        //public void JsonNull2Test()
        //{
        //    var t = new NullTestClass2();
        //    Include<NullTestClass2> includes = (navigation)=> navigation.Include(e=>e.NullTestClassParent);
        //    var json1 = includes.SerializeJson(t, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text1 = JsonConvert.SerializeObject(t, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json1 != text1)
        //        throw new Exception("test 1");

        //    var json2 = includes.SerializeJson(t, new NExpJsonSerializerSettings() { NullValueHandling = false });
        //    string text2 = JsonConvert.SerializeObject(t, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    if (json2 != text2)
        //        throw new Exception("test 2");

        //    t.Int1NullableValue = null;
        //    var json3 = includes.SerializeJson(t, new NExpJsonSerializerSettings() { NullValueHandling = false });
        //    string text3 = JsonConvert.SerializeObject(t, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    if (json3 != text3)
        //        throw new Exception("test 3");


        //    var codes = default(byte[]);
        //    Include<byte[]> includes4 = null;
        //    var json4 = includes4.SerializeJson(codes, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4 = JsonConvert.SerializeObject(codes, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4 != text4)
        //        throw new Exception("test 3");
        //    var json5 = includes4.SerializeJson(codes, new NExpJsonSerializerSettings() { NullValueHandling = false });
        //    string text5 = JsonConvert.SerializeObject(codes, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    if (json5 != text5)
        //        throw new Exception("test 3");

        //}

        //[TestMethod]
        //public void JsonPrimitiveTypesTest()
        //{
        //    var str = default(string);
        //    Include<string> includes = null;
        //    var json1 = includes.SerializeJson(str, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text1 = JsonConvert.SerializeObject(str, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json1 != text1)
        //        throw new Exception("test 1");

        //    var strN = "";
        //    Include<string> includesN = null;
        //    var json1N = includesN.SerializeJson(strN, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text1N = JsonConvert.SerializeObject(strN, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json1N != text1N)
        //        throw new Exception("test 1");

        //    var str1 = "abc";
        //    var json1b = includes.SerializeJson(str1, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text1b = JsonConvert.SerializeObject(str1, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json1b != text1b)
        //        throw new Exception("test 1");

        //    var json2 = includes.SerializeJson(str, new NExpJsonSerializerSettings() { NullValueHandling = false });
        //    string text2 = JsonConvert.SerializeObject(str, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    if (json2 != text2)
        //        throw new Exception("test 2");

        //    var number = 5;
        //    Include<int> includes2 = null;
        //    var json3 = includes2.SerializeJson(number, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text3 = JsonConvert.SerializeObject(number, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json3 != text3)
        //        throw new Exception("test 3");

        //    var ints = new int[] { 0, 1, 2 };
        //    Include<int> includes4 = null;
        //    var json4 = NExpJsonExtensions.SerializeJson(includes4, ints, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4 = JsonConvert.SerializeObject(ints, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4 != text4)
        //        throw new Exception("test 4");

        //    var strs = new string[] { "0", "1", "2", null };
        //    Include<string> includes4s = null;
        //    var json4s = NExpJsonExtensions.SerializeJson(includes4s, strs, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4s = JsonConvert.SerializeObject(strs, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4s != text4s)
        //        throw new Exception("test 4s");

        //    var strse = new string[] { };
        //    Include<string> includes4se = null;
        //    var json4se = NExpJsonExtensions.SerializeJson(includes4se, strse, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4se = JsonConvert.SerializeObject(strse, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4se != text4se)
        //        throw new Exception("test 4se");

        //    var strses = new string[][] { new string[]{ } };
        //    Include<string> includes4ses = null;
        //    var json4ses = NExpJsonExtensions.SerializeJson(includes4ses, strses, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4ses = JsonConvert.SerializeObject(strses, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4ses != text4ses)
        //        throw new Exception("test 4ses");

        //    var strses3 = new string[][] { new string[] {""} };
        //    Include<string> includes4ses3 = null;
        //    var json4ses3 = NExpJsonExtensions.SerializeJson(includes4ses, strses3, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4ses3 = JsonConvert.SerializeObject(strses3, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4ses3 != text4ses3)
        //        throw new Exception("test 4ses");

        //    var strses2 = new long?[][] { new long?[] {null, 5,2 }, null, new long?[] { null, -1, 0 } };
        //    Include<long?> includes4ses2 = null;
        //    var json4ses2 = NExpJsonExtensions.SerializeJson(includes4ses2, strses2, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text4ses2 = JsonConvert.SerializeObject(strses2, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4ses2 != text4ses2)
        //        throw new Exception("test 4ses");

        //    var bytes = new byte[] { 0, 1, 2 };
        //    Include<byte[]> includes7 = null;
        //    var json7 = includes7.SerializeJson(bytes, new NExpJsonSerializerSettings() { NullValueHandling = true });
        //    string text7 = JsonConvert.SerializeObject(bytes, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        //    if (json4 != text4)
        //        throw new Exception("test4");
        //}
    }
}