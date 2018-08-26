using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.ActiveDirectoryServices
{
    public class FakeAdConfiguration 
    {
        public string FakeAdUser { get; set; }
        public List<string> FakeAdGroups { get; set; } = new List<string>();
    }
}