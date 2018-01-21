using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1
{
    public interface IAdmikaConfigurationFactory
    {
        ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for);
    }

    public interface IAdmikaConfigurationFacade: IAdmikaConfigurationFactory
    {
        AdminkaStorageConfiguration ResolveAdminkaStorageConfiguration();
    }
}