﻿using Microsoft.Extensions.Configuration;
using System;
using Vse.Routines.Configuration;
using Vse.Routines.Configuration.NETStandard;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
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
            return RoutinesConfigurationManager.GetConfigurationContainer(ConfigurationRoot, @namespace, @class, member);
        }

        public string GetConnectionString()
        {
            return RoutinesConfigurationManager.GetConnectionString(ConfigurationRoot, "adminka");
        }
    }
}