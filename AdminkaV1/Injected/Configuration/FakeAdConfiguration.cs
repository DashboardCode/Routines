using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Configuration
{
    public class FakeAdConfiguration : System.IProgress<string>
    {
        public string FakeAdUser { get; set; }
        public List<string> FakeAdGroups { get; set; } = new List<string>();

        public void Report(string json)
        {
            var dictionary = InjectedManager.DeserializeJson<FakeAdConfiguration>(json);
            FakeAdUser = dictionary.FakeAdUser;
            FakeAdGroups = dictionary.FakeAdGroups;
        }
    }
}