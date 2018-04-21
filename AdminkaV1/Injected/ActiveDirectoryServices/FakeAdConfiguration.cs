using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.ActiveDirectoryServices
{
    public class FakeAdConfiguration : System.IProgress<string>
    {
        public string FakeAdUser { get; set; }
        public List<string> FakeAdGroups { get; set; } = new List<string>();

        public void Report(string json)
        {
            var fakeAdConfiguration = InjectedManager.DeserializeJson<FakeAdConfiguration>(json);
            FakeAdUser = fakeAdConfiguration.FakeAdUser;
            FakeAdGroups = fakeAdConfiguration.FakeAdGroups;
        }
    }
}