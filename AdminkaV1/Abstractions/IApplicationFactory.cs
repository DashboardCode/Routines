using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1
{
    public interface IApplicationFactory
    {
        AdminkaStorageConfiguration CreateAdminkaStorageConfiguration();
        ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for);
    }
}