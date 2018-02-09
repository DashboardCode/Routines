using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Xunit;

using DashboardCode.Routines.Configuration.NETStandard;

[assembly: CollectionBehavior(MaxParallelThreads = 1, DisableTestParallelization = true)]

namespace DashboardCode.Routines.Configuration.NETCore.Test
{
    public class ConfigurationNETStandard
    {
        /// <summary>
        /// Recreate file in test folder (need for OpenCover)
        /// </summary>
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

        readonly IConfigurationRoot configurationRoot;
        readonly ConfigurationManagerLoader configurationManagerLoader;
        public ConfigurationNETStandard()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            configurationRoot = configurationBuilder.Build();
            configurationManagerLoader = new ConfigurationManagerLoader(configurationRoot);
        }

        public ConfigurationContainer Create(MemberTag memberTag) =>
            new ConfigurationContainer(configurationManagerLoader, memberTag);
    }
}