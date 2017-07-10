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
            var source1 = TestTool.CreateTestModel();
            var include1 = TestTool.CreateInclude();

            // TODO: 1) add nice error message 2) add string[] formatter
            var formatter = JsonChainNodeTools.BuildFormatter(include1,
                    (n, b) => JsonChainNodeTools.GetDefaultSerializerSet(n, b)
            );
            var json = formatter(source1);

        }
    }
}
