using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Routines.Json.Test
{
    // Exception Newtonsoft.Json.JsonSerializationException: 'Self referencing loop detected for property '..' with type  '..'

    [TestClass]
    public class JsonNetTest
    {
        public abstract class Business
        {
            public string Name { get; set; }
        }

        public class Hotel : Business
        {
            public int Stars { get; set; }
        }

        public class Stockholder
        {
            public string FullName { get; set; }
            public IList<Business> Businesses { get; set; }
        }

        [TestMethod]
        public void TestJsonVector()
        {
            var p = new System.Drawing.Point(0, 0);
            var json = $"{{\"x\":{p.X}, \"y\":{p.Y}}}";
        }
        [TestMethod]
        public void TestJsonNetCircular()
        {
            Stockholder stockholder = new Stockholder
            {
                FullName = "Steve Stockholder",
                Businesses = new List<Business>
                {
                    new Hotel
                    {
                        Name = "Hudson Hotel",
                        Stars = 4
                    }
                }
            };

            string jsonTypeNameAll = JsonConvert.SerializeObject(stockholder, Formatting.Indented, new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.All
            });






            //var settings = new JsonSerializerSettings{
            //    ContractResolver = new RoutinesContractResolver(),

            //};
            //var t1 = TestTool.CreateTestModel();
            //var o1 = JsonConvert.SerializeObject(t1, settings);

            //var t2 = new {i = "i", j=5 };
            //var o2 = JsonConvert.SerializeObject(t2, settings);
        }
    }
}
