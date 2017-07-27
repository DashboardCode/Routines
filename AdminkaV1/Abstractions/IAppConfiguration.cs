using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1
{
    public enum StorageType { SQLSERVER, INMEMORY }
    public interface IAppConfiguration
    {
        string GetConnectionString();
        string GetMigrationAssembly();
        StorageType GetStorageType();
        SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member);
    }
}
