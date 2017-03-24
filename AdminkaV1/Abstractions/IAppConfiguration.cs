using Vse.Routines.Configuration;

namespace Vse.AdminkaV1
{
    public interface IAppConfiguration
    {
        string GetConnectionString();
        SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member);
    }
}
