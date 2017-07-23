using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1
{
    public interface IAppConfiguration
    {
        string GetConnectionString();
        SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member);
    }
}
