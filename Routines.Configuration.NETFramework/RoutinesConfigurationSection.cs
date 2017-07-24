using System;
using System.Configuration;

namespace DashboardCode.Routines.Configuration.NETFramework
{
    public class RoutinesConfigurationSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty routinesCollectionProperty 
            = new ConfigurationProperty("",typeof(RoutineElementCollection),null,ConfigurationPropertyOptions.IsDefaultCollection);
        private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection{routinesCollectionProperty};
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public RoutineElementCollection Routines
        {
            get
            {
                return (RoutineElementCollection)base[routinesCollectionProperty];
            }
        }

        public string GetXml(string sectionName)
        {
            return this.SerializeSection(null, sectionName, ConfigurationSaveMode.Full);
        }
        #region Debug
        public static DateTime StaticConstructedAt { get; } = DateTime.Now;
        public DateTime ConstructedAt { get; } = DateTime.Now;

        public override string ToString()
        {
            var text = $"{ConstructedAt}/{StaticConstructedAt} " +  base.ToString();
            foreach (var r in Routines)
            {
                text = text + "; " + r.ToString();
            }
            return text;
        }
        #endregion
    }
}
