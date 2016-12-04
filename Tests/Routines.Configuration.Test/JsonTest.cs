using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ServiceStack;

namespace Vse.Routines.Configuration.Test
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void NewtonsoftJsonSerialize()
        {
            var t = new LoggingConfiguration() { StartActivity=true, FinishActivity = true, Input = true, Output = false, Verbose = true, UseBufferForVerbose = true, VerboseWithStackTrace = true };
            var serialized = JsonConvert.SerializeObject(t);
            if (serialized == null)
                throw new ApplicationException("Test fails");
        }

        [TestMethod]
        public void SystemRuntimeSerializationJsonSerialize()
        {
            var t = new LoggingConfiguration() { StartActivity = true, FinishActivity = true, Input = true, Output = false, Verbose = true, UseBufferForVerbose = true, VerboseWithStackTrace = true };
            var serialized = default(string);
            var serializer = new DataContractJsonSerializer(typeof(LoggingConfiguration), new DataContractJsonSerializerSettings(){UseSimpleDictionaryFormat=true});
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, t);
                serialized = Encoding.Default.GetString(stream.ToArray());
            }
        }

        [TestMethod]
        public void NewtonsoftJsonDeserialize()
        {
            var serialized = "{StartActivity:true, FinishActivity:true, Input:true, Output:false, Verbose:true, UseBufferForVerbose:true, VerboseWithStackTrace:true }";
            var t = JsonConvert.DeserializeObject<LoggingConfiguration>(serialized);
            if (t == null)
                throw new ApplicationException("Test fails");
        }


        [TestMethod]
        public void SystemRuntimeSerializationJsonDeserialize()
        {
            var serialized = "{\"StartActivity\":true, \"FinishActivity\":true, \"Input\":true, \"Output\":false, \"Verbose\":true, \"UseBufferForVerbose\":true, \"VerboseWithStackTrace\":true }";
            var t = default(LoggingConfiguration);
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(serialized)))
            {
                var deserializer = new DataContractJsonSerializer(
                    typeof(LoggingConfiguration),
                    new DataContractJsonSerializerSettings() { });
                t = (LoggingConfiguration)deserializer.ReadObject(ms);
            }
        }

        [TestMethod]
        public void ServiceStackDynamicJsonDeserialize()
        {
            var serialized = "{\"StartActivity\":true, \"FinishActivity\":true, \"Input\":true, \"Output\":false, \"Verbose\":true, \"UseBufferForVerbose\":true, \"VerboseWithStackTrace\":true }";
            var l = new LoggingConfiguration();
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
        public void ServiceStackJsonDeserialize()
        {
            var serialized = "{\"StartActivity\":true, \"FinishActivity\":true, \"Input\":true, \"Output\":false, \"Verbose\":true, \"UseBufferForVerbose\":true, \"VerboseWithStackTrace\":true }";
            var t = serialized.FromJson<LoggingConfiguration>();
        }

        [TestMethod]
        public void SystemWebExtensionJsonDeserialize()
        {
            //var serialized = "[['asd', 'pok'],['asd2', 'pok2']]";
            var serialized = "{StartActivity:true, FinishActivity:true, Input:true, Output:false, Verbose:true, UseBufferForVerbose:true, VerboseWithStackTrace:true }";
            var jss = new JavaScriptSerializer();
            var t = jss.Deserialize<LoggingConfiguration>(serialized);
        }

        [TestMethod]
        public void SystemWebExtensionJsonSerialize()
        {
            var jss = new JavaScriptSerializer();
            
            var array1 = jss.Serialize(new[] {"asd","pok"});
            var array2 = jss.Serialize(new[] { new[] { "asd", "pok" }, new[] { "asd2", "pok2" } });
        }
    }
}
