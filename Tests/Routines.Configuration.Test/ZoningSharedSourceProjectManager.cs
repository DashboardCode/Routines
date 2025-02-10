
using System;
using System.Collections.Generic;
#if NET9_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif

namespace DashboardCode.Routines.Configuration.Test
{
    public static class ZoningSharedSourceProjectManager
    {
#if NET9_0_OR_GREATER
        public static IConfigurationManagerLoader<IConfigurationSection> GetLoader()
        {
            var configurationBuilder = new ConfigurationBuilder();
            JsonConfigurationExtensions.AddJsonFile(configurationBuilder, "appsettings.json", false, true);
            var configurationRoot = configurationBuilder.Build();
            return new Standard.ConfigurationManagerLoader(configurationRoot);
        }
#else
        public static IConfigurationManagerLoader<string> GetLoader()
        {
            return new Classic.ConfigurationManagerLoader();
        }
#endif
    }

#if NET9_0_OR_GREATER
    public class Deserializer : IGWithConstructorFactory<IConfigurationSection>
    {
        public TOutput Create<TOutput>(IConfigurationSection section) where TOutput : new()
        {
            var t = new TOutput();
            if (t is IProgress<Dictionary<string, string>>)
            {
                if (section != null)
                {
                    var dictionary = new Dictionary<string, string>();
                    section.Bind(dictionary);
                    ((IProgress<Dictionary<string, string>>)t).Report(dictionary);
                }

            }
            else
            {
                if (section != null)
                {
                    section.Bind(t);
                }
            }
            return t;
        }
    }

    public class ConfigurationContainerTest : ConfigurationContainer<IConfigurationSection>
    {
        public ConfigurationContainerTest(IEnumerable<IRoutineConfigurationRecord<IConfigurationSection>> routineConfigurationRecords,
            IGWithConstructorFactory<IConfigurationSection> deserializer,
            MemberTag memberTag) : base(routineConfigurationRecords, deserializer, memberTag)
        {
        }

        public ConfigurationContainerTest(IEnumerable<IRoutineConfigurationRecord<IConfigurationSection>> routineConfigurationRecords,
            IGWithConstructorFactory<IConfigurationSection> deserializer,
            MemberTag memberTag, string @for) : base(routineConfigurationRecords, deserializer,  memberTag, @for)
        {
        }
    }

#else
    public class Deserializer : IGWithConstructorFactory<string>
    {
        public TOutput Create<TOutput>(string input) where TOutput : new()
        {
            var t = new TOutput();
            if (t is IProgress<Dictionary<string, string>>)
            {
                var dictionary = StaticTools.DeserializeJson<Dictionary<string, string>>(input);
                ((IProgress<Dictionary<string, string>>)t).Report(dictionary);
            }
            else
            {
                t = StaticTools.DeserializeJson<TOutput>(input);
            }
            return t;
        }
    }

    public class ConfigurationContainerTest : ConfigurationContainer<string>
    {
        public ConfigurationContainerTest(IEnumerable<IRoutineConfigurationRecord<string>> routineConfigurationRecords, 
            IGWithConstructorFactory<string> deserializer,
            MemberTag memberTag) : base(routineConfigurationRecords,  deserializer, memberTag)
        {
        }

        public ConfigurationContainerTest(IEnumerable<IRoutineConfigurationRecord<string>> routineConfigurationRecords,
            IGWithConstructorFactory<string> deserializer,
            MemberTag memberTag, string @for) : base(routineConfigurationRecords, deserializer, memberTag, @for)
        {
        }
    }
#endif
}