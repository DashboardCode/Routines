using System.IO;
using System.Reflection;
using Xunit;

using DashboardCode.Routines.Configuration.Standard;
using DashboardCode.Routines.Configuration.Test;

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

        readonly ConfigurationManagerLoader configurationManagerLoader;
        public ConfigurationNETStandard()
        {
            var configurationRoot = ConfigurationManager.ResolveConfigurationRoot();
            configurationManagerLoader = new ConfigurationManagerLoader(configurationRoot, new Deserializer());
        }

        public ConfigurationContainer<Microsoft.Extensions.Configuration.IConfigurationSection> Create(MemberTag memberTag) =>
            new ConfigurationContainer<Microsoft.Extensions.Configuration.IConfigurationSection>(
                configurationManagerLoader.GetGetRoutineConfigurationRecords(),
                 new Deserializer(),
                memberTag);
    }
}