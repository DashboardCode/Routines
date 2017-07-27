using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using System;

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
        public SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member)
        {
            return RoutinesConfigurationManager.CreateConfigurationContainer(ConfigurationRoot, @namespace, @class, member);
        }

        public string GetConnectionString()
        {
            return RoutinesConfigurationManager.GetConnectionString(ConfigurationRoot, "adminka");
        }

        public string GetMigrationAssembly()
        {
            return "DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp";
        }

        public StorageType GetStorageType()
        {
            return StorageType.SQLSERVER;
        }
    }
}
