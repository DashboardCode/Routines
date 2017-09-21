using DashboardCode.Routines;
using DashboardCode.Routines.IncludableTypes;

namespace DashboardCode.AdminkaV1.AuthenticationDom.Includes
{
    [Extends(definitionType: typeof(UserInclude), definitionInculde: nameof(DtoDefinition))]
    public partial class UserInclude
    {
        public static readonly Include<AuthenticationDom.User> DtoDefinition
            = (i) => i.Include(e => e.UserId)
                      .Include(e => e.LoginName)
                      .Include(e => e.FirstName)
                      .Include(e => e.SecondName)
                      .IncludeAll(e => e.UserPrivilegeMap)
                        .ThenInclude(e => e.Privilege)
                            .ThenInclude(e => e.PrivilegeId)
                      .IncludeAll(e => e.UserPrivilegeMap)
                         .ThenInclude(e => e.Privilege)
                            .ThenInclude(e => e.PrivilegeName);
    }
}
