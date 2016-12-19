
using Vse.Routines;
using Vse.Routines.IncludableTypes;

namespace Vse.AdminkaV1.Web.DTO
{
    [Extends(definitionType: typeof(UserDto), definitionInculde: nameof(Definition))]
    public partial struct UserDto
    {
        public static readonly Include<DomAuthentication.User> Definition
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
