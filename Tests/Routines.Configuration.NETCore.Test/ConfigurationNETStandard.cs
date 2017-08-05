using DashboardCode.Routines.Configuration.NETStandard;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using Xunit;

[assembly: CollectionBehavior(MaxParallelThreads = 1, DisableTestParallelization = true)]

namespace DashboardCode.Routines.Configuration.NETCore.Test
{
    public class ConfigurationNETStandard
    {
        static ConfigurationNETStandard(){
            // create  configuration file
            string fileName = "appsettings.json";
            var entryAssembly = typeof(ConfigurationNETStandard).GetTypeInfo().Assembly; // check also Assembly.GetEntryAssembly() or Assembly.GetExecutingAssembly()
            var type = typeof(ConfigurationNETStandard);
            using (var reader = new StreamReader(entryAssembly
                .GetManifestResourceStream(type.Namespace+"."+fileName)))
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                        reader.BaseStream.CopyTo(fileStream);
        }

        private readonly IConfigurationRoot configurationRoot;
        public ConfigurationNETStandard()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            configurationRoot = configurationBuilder.Build();
        }

        public SpecifiableConfigurationContainer GetSpecifiableConfigurationContainer(MemberTag memberTag) =>
            RoutinesConfigurationManager.CreateConfigurationContainer(configurationRoot, memberTag);
    }
}
