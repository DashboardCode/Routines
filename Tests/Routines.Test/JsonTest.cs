using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Vse.Routines.Json;


namespace Vse.Routines.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void JsonSerializeTest()
        {
            var t = TestTool.CreateTestModel();
            var includes = TestTool.CreateIncludeWithoutLeafs();
            var serializer = includes.BuildNavigationExpressionJsonSerializer();
            //var tasks = new List<Task>();
            //for (int i = 1; i <= 16; i++ )
            //{
            //    tasks.Add(Task.Run(
            //        () =>
            //        {
            //            for (int j = 1; j < 100000; j++)
            //            {
                            var json = includes.SerializeJson(t, serializer);
                                var x = @"{""StorageModel"":{""Entity"":{""Namespace"":""EntityNamespace1"",""Name"":""EntityName1""},""Key"":{""Attributes"":{}},""TableName"":""TableName1"",""Uniques"":[{""IndexName"":""IndexName1"",""Fields"":[""FieldU1""]},{""IndexName"":""IndexName2"",""Fields"":[""FieldU2""]}]},""Test"":{},""ListTest"":{},""Message"":{""TextMsg"":""Initial"",""DateTimeMsg"":{},""IntNullableMsg"":7},""IntNullable2"":555}";
                            if (json != @"{""StorageModel"":{""Entity"":{""Name"":""EntityName1"",""Namespace"":""EntityNamespace1""},""Key"":{""Attributes"":{}},""TableName"":""TableName1"",""Uniques"":[{""IndexName"":""IndexName1"",""Fields"":[""FieldU1""]},{""IndexName"":""IndexName2"",""Fields"":[""FieldU2""]}]},""Test"":{},""ListTest"":{},""Message"":{""TextMsg"":""Initial"",""DateTimeMsg"":{},""IntNullableMsg"":7},""IntNullable2"":555}")
                                throw new Exception("x");
            //            }
            //        }));
            //}
            //Task.WaitAll(tasks.ToArray());
        }
    }
}
