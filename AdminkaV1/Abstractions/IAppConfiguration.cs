using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1
{
    public enum StorageType { SQLSERVER, INMEMORY }
    public interface IAppConfiguration
    {
        string ResolveConnectionString();
        string ResolveMigrationAssembly();
        StorageType ResolveStorageType();
        SpecifiableConfigurationContainer ResolveConfigurationContainer(MemberTag memberTag);
    }
}