using System;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;

namespace DashboardCode.AdminkaV1.WcfService
{
    public class ConfigurationNETFramework : IAppConfiguration
    {
        public SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member)=>
            RoutinesConfigurationManager.GetConfigurationContainer(@namespace, @class, member);

        public string GetConnectionString() =>
            RoutinesConfigurationManager.GetConnectionString("adminka");

        public string GetMigrationAssembly()
            => null;

        public StorageType GetStorageType() =>
            StorageType.INMEMORY;
    }
}