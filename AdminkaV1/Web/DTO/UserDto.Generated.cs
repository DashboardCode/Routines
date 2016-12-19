using System.Collections.Generic;
using Vse.Routines;

namespace Vse.AdminkaV1.Web.DTO
{
    partial struct UserDto
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public IReadOnlyCollection<_UserPrivilegeMap> UserPrivilegeMap { get; set; }

        public struct _UserPrivilegeMap
        {
            public _Privilege Privilege { get; set; }
        }

        public struct _Privilege
        {
            public int PrivilegeId { get; set; }
            public string PrivilegeName { get; set; }
        }
    }
    
    public static class UserDtoExtnesions
    {
        public static readonly Include<UserDto> Statement
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
        public static DomAuthentication.User Cast(this UserDto userDto)
        {
            var user = new DomAuthentication.User();
            MemberExpressionExtensions.Cast(userDto, user, Statement);
            return user;
        }
        public static UserDto Cast(this DomAuthentication.User user)
        {
            var userDto = new UserDto();
            MemberExpressionExtensions.Cast(user, userDto, UserDto.Definition);
            return userDto;
        }
    }
}
