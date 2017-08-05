using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class InstallerConfiguration : IAppConfiguration
    {
        public IConfigurationRoot ConfigurationRoot  { get; private set;}
        public InstallerConfiguration()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.ConfigurationRoot = configurationBuilder.Build();
        }
        public SpecifiableConfigurationContainer ResolveConfigurationContainer(MemberTag memberTag)
        {
            return RoutinesConfigurationManager.CreateConfigurationContainer(ConfigurationRoot, memberTag);
        }

        public string ResolveConnectionString()
        {
            return RoutinesConfigurationManager.MakeConnectionString(ConfigurationRoot, "adminka");
        }

        public string ResolveMigrationAssembly()
        {
            return "DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp";
        }

        public StorageType ResolveStorageType()
        {
            return StorageType.SQLSERVER;
        }
    }
}
