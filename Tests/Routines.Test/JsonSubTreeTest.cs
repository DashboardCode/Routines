using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.Test
{
    class MyClass
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
    }

    [TestClass]
    public class JsonSubTreeTest
    {
        [TestMethod]
        public void JsonSerializeSunsetString()
        {
            var myClass = new MyClass() { Text1="Text1", Text2="{\"Text2\":\"Text2\"}"};
             
            Include<MyClass> include = chain => chain.Include(e=>e.Text1).Include(e => e.Text2);

            var formatter1 = JsonManager.ComposeFormatter(
                include
            );

            var formatter2 = JsonManager.ComposeFormatter(
                include,
                rules => rules
                    .SubTree(
                          chain => chain.Include(e => e.Text2),
                          stringAsJsonLiteral: true
                    )
            );

            var formatter3 = JsonManager.ComposeFormatter(
                include,
                rules => rules
                    .SubTree(
                          chain => chain.Include(e => e.Text2), 
                          stringJsonEscape:false
                    )
            );

            var json1 = formatter1(myClass);
            var json2 = formatter2(myClass);
            var json3 = formatter3(myClass);
            if (json1 != "{\"Text1\":\"Text1\",\"Text2\":\"{\\\"Text2\\\":\\\"Text2\\\"}\"}")
                throw new Exception(nameof(JsonSerializeSunsetString) + " 1");
            if (json2 != "{\"Text1\":\"Text1\",\"Text2\":{\"Text2\":\"Text2\"}}")
                throw new Exception(nameof(JsonSerializeSunsetString) + " 2");
            if (json3 != "{\"Text1\":\"Text1\",\"Text2\":\"{\"Text2\":\"Text2\"}\"}")
                throw new Exception(nameof(JsonSerializeSunsetString) + " 3");
        }
    }
}

