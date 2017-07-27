using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    public static class RoutinesConfigurationManager
    {
        const string defaultFileName    = "appsettings.json";
        const string defaultSectionName = "Routines";
        public static SpecifiableConfigurationContainer CreateConfigurationContainer(
            this IConfigurationRoot configurationRoot,
            string @namespace, string @class, string member, string fileName = defaultFileName,  string sectionName = defaultSectionName)
        {
            var section = configurationRoot.GetSection(sectionName);
            var routineResolvables = new List<RoutineResolvable>();
            section.Bind(routineResolvables);
            var ranged = RoutinesExtensions.RangedRoutines(routineResolvables, @namespace, @class, member);
            var @container = new SpecifiableConfigurationContainer(ranged);
            return @container;
        }
        public static string GetConnectionString(this IConfigurationRoot configurationRoot, string name)
        {
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;
            return connectionString;
        }
    }
}
