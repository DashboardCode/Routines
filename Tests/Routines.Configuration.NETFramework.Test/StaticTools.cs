using System.Web.Script.Serialization;

namespace DashboardCode.Routines.Configuration.NETFramework.Test
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
