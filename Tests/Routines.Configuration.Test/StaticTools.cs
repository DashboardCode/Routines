namespace DashboardCode.Routines.Configuration.Test
{
    public static class StaticTools
    {
        public static  T DeserializeJson<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
