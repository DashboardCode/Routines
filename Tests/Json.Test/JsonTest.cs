using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ServiceStack;

namespace Vse.Json.Test
{
    [TestClass]
    public class JsonTest
    {
        /// <summary>
        /// It is a best deserializer not only because of speed (2nd) but also because of supported fluent
        /// syntax. It supports also single quotes what is important mixing xml and json, c# and json.
        /// E.g it is possible to deserialize <code>var serialized = "[['asd', 'pok'],['asd2', 'pok2']]";</code>
        /// </summary>
        [TestMethod]
        public void Deserialize_SystemWebExtensionJson()
        {
            
            var serialized = "{StartActivity:true, FinishActivity:true, Input:true, Output:false, Verbose:true, UseBufferForVerbose:true, VerboseWithStackTrace:true }";
            var jss = new JavaScriptSerializer();
            var t = jss.Deserialize<TestStructure>(serialized);
        }

        [TestMethod]
        public void Serialize_NewtonsoftJson()
        {
            var t = new TestStructure() { StartActivity=true, FinishActivity = true, Input = true, Output = false, Verbose = true, UseBufferForVerbose = true, VerboseWithStackTrace = true };
            var serialized = JsonConvert.SerializeObject(t);
            if (serialized == null)
                throw new ApplicationException("Test fails");
        }

        [TestMethod]
        public void Serialize_SystemRuntimeSerializationJson()
        {
            var t = new TestStructure() { StartActivity = true, FinishActivity = true, Input = true, Output = false, Verbose = true, UseBufferForVerbose = true, VerboseWithStackTrace = true };
            var serialized = default(string);
            var serializer = new DataContractJsonSerializer(typeof(TestStructure), new DataContractJsonSerializerSettings(){UseSimpleDictionaryFormat=true});
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, t);
                serialized = Encoding.Default.GetString(stream.ToArray());
            }
        }

        [TestMethod]
        public void Deserialize_NewtonsoftJson()
        {
            var serialized = "{StartActivity:true, FinishActivity:true, Input:true, Output:false, Verbose:true, UseBufferForVerbose:true, VerboseWithStackTrace:true }";
            var t = JsonConvert.DeserializeObject<TestStructure>(serialized);
            if (t == null)
                throw new ApplicationException("Test fails");
        }


        [TestMethod]
        public void Deserialize_SystemRuntimeSerializationJson()
        {
            var serialized = "{\"StartActivity\":true, \"FinishActivity\":true, \"Input\":true, \"Output\":false, \"Verbose\":true, \"UseBufferForVerbose\":true, \"VerboseWithStackTrace\":true }";
            var t = default(TestStructure);
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serialized)))
            {
                var deserializer = new DataContractJsonSerializer(
                    typeof(TestStructure),
                    new DataContractJsonSerializerSettings() { });
                t = (TestStructure)deserializer.ReadObject(ms);
            }
        }

        [TestMethod]
        public void Deserialize_ServiceStackDynamicJson()
        {
            var serialized = "{\"StartActivity\":true, \"FinishActivity\":true, \"Input\":true, \"Output\":false, \"Verbose\":true, \"UseBufferForVerbose\":true, \"VerboseWithStackTrace\":true }";
            var l = new TestStructure();
            dynamic d = DynamicJson.Deserialize(serialized);
            l.StartActivity = bool.Parse(d.StartActivity);
            l.FinishActivity = bool.Parse(d.FinishActivity);
            l.Input = bool.Parse(d.Input);
            l.Output = bool.Parse(d.Output);
            l.Verbose = bool.Parse(d.Verbose);
            l.UseBufferForVerbose = bool.Parse(d.UseBufferForVerbose);
            l.VerboseWithStackTrace = bool.Parse(d.VerboseWithStackTrace);
            l.VerboseWithStackTrace = bool.Parse(d.VerboseWithStackTrace);
        }

        [TestMethod]
        public void Deserialize_ServiceStackJson()
        {
            var serialized = "{\"StartActivity\":true, \"FinishActivity\":true, \"Input\":true, \"Output\":false, \"Verbose\":true, \"UseBufferForVerbose\":true, \"VerboseWithStackTrace\":true }";
            var t = serialized.FromJson<TestStructure>();
        }


        [TestMethod]
        public void Serialize_SystemWebExtensionJson()
        {
            var jss = new JavaScriptSerializer();
            
            var array1 = jss.Serialize(new[] {"asd","pok"});
            var array2 = jss.Serialize(new[] { new[] { "asd", "pok" }, new[] { "asd2", "pok2" } });
        }
    }
}
