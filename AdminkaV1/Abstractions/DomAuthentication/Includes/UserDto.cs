using Vse.Routines;
using Vse.Routines.IncludableTypes;

namespace Vse.AdminkaV1.DomAuthentication.Includes
{
    [Extends(definitionType: typeof(UserInclude), definitionInculde: nameof(DtoDefinition))]
    public partial class UserInclude
    {
        public static readonly Include<DomAuthentication.User> DtoDefinition
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
