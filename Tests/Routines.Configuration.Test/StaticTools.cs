using System.Web.Script.Serialization;

namespace Vse.Routines.Configuration.Test
{
    
    public static class StaticTools
    {
        public static  T DeserializeJson<T>(string json)
        {
            var jss = new JavaScriptSerializer();
            var t = jss.Deserialize<T>(json);
            return t;
        }
    }
}
