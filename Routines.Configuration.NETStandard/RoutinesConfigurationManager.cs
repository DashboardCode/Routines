using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Vse.Routines.Configuration.NETStandard
{
    public class RoutinesConfigurationManager
    {
        const string defaultFileName = "appsettings.json";
        const string defaultSectionName = "Routines";
        public static SpecifiableConfigurationContainer GetConfigurationContainer(
            IConfigurationRoot configurationRoot,
            string @namespace, string @class, string member, string fileName = defaultFileName,  string sectionName = defaultSectionName)
        {
            var section = configurationRoot.GetSection(sectionName);
            var routineResolvables = new List<RoutineResolvable>();
            section.Bind(routineResolvables);
            var ranged = RoutinesExtensions.RangedRoutines(routineResolvables, @namespace, @class, member);
            var container = new SpecifiableConfigurationContainer(ranged);
            return container;
        }
        public static string GetConnectionString(IConfigurationRoot configurationRoot, string name)
        {
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;
            return connectionString;
        }
    }
}
