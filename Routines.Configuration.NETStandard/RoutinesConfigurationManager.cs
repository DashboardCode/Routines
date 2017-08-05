using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration.NETStandard
{
    public static class RoutinesConfigurationManager
    {
        const string defaultFileName    = "appsettings.json";
        const string defaultSectionName = "Routines";
        public static ISpecifiableConfigurationContainer CreateConfigurationContainer(
            this IConfigurationRoot configurationRoot,
            MemberTag memberTag, string fileName = defaultFileName,  string sectionName = defaultSectionName)
        {
            var section = configurationRoot.GetSection(sectionName);
            var routineResolvables = new List<RoutineResolvable>();
            section.Bind(routineResolvables);
            var ranged = RoutinesExtensions.RangedRoutines(routineResolvables, memberTag);
            var @container = new SpecifiableConfigurationContainer(ranged);
            return @container;
        }
        public static string MakeConnectionString(this IConfigurationRoot configurationRoot, string name)
        {
            var connectionString = configurationRoot.GetSection("ConnectionString").Value;
            return connectionString;
        }
    }
}
