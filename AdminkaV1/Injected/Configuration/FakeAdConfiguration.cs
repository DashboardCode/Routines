using System.Collections.Generic;

namespace Vse.AdminkaV1.Injected.Configuration
{
    public class FakeAdConfiguration : System.IProgress<string>
    {
        public string FakeAdUser { get; set; }
        public List<string> FakeAdGroups { get; set; } = new List<string>();

        public void Report(string json)
        {
            var dictionary = IoCManager.DeserializeJson<FakeAdConfiguration>(json);
            FakeAdUser = dictionary.FakeAdUser;
            FakeAdGroups = dictionary.FakeAdGroups;
        }
    }
}
