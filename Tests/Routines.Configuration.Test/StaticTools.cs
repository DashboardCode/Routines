#if !NETCOREAPP1_1
using System.Web.Script.Serialization;
#endif
namespace DashboardCode.Routines.Configuration.Test
{
    public static class StaticTools
    {
        public static  T DeserializeJson<T>(string json)
        {
            #if !NETCOREAPP1_1
            var jss = new JavaScriptSerializer();
            var t = jss.Deserialize<T>(json);
            return t;
#else
                throw new System.Exception("Not implemented");
#endif

        }
    }
}
