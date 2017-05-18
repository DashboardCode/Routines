using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.Routines.Json;

namespace Vse.Routines.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void JsonSerializeTest()
        {
            var t = TestTool.CreateTestModelWithNulls();
            var includes = TestTool.CreateIncludeWithoutLeafs();
            var settings = new NExpJsonSerializerSettings() {
                NullValueHandling=true
            };
            var serializer = includes.BuildNExpJsonSerializer(settings);
            var json = includes.SerializeJson(t, serializer);
        }
    }
}
