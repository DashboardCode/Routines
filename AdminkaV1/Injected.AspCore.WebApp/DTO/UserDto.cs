using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public List<UserPrivilegeMapValue> UserPrivilegeMap { get; set; }

        public class UserPrivilegeMapValue
        {
            public PrivilegeValue Privilege { get; set; }
        }

        public class PrivilegeValue
        {
            public string PrivilegeId { get; set; }
            public string PrivilegeName { get; set; }
        }
    }

    public static class UserDtoExtnesions
    {
        public static AuthenticationDom.User Cast(this UserDto userDto)
        {
            var user = new AuthenticationDom.User() {
                UserId = userDto.UserId,
                LoginName = userDto.LoginName,
                FirstName = userDto.FirstName,
                SecondName = userDto.SecondName,
            };
            if (userDto.UserPrivilegeMap!=null)
                user.UserPrivilegeMap = new List<AuthenticationDom.UserPrivilege>();
            foreach (var userPrivilegeMap in userDto.UserPrivilegeMap)
            {
                if (userPrivilegeMap != null)
                {
                    var userPrivilege = new AuthenticationDom.UserPrivilege();
                    if (userPrivilegeMap.Privilege != null)
                    {
                        userPrivilege.Privilege = new AuthenticationDom.Privilege()
                        {
                            PrivilegeId = userPrivilegeMap.Privilege.PrivilegeId,
                            PrivilegeName = userPrivilegeMap.Privilege.PrivilegeName
                        };
                    }
                    user.UserPrivilegeMap.Add(userPrivilege);
                }
            }
            return user;
        }

        public static UserDto Cast(this AuthenticationDom.User user)
        {
            var userDto = new UserDto(){
                UserId = user.UserId,
                LoginName = user.LoginName,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
            };
            if (user.UserPrivilegeMap != null)
                userDto.UserPrivilegeMap = new List<UserDto.UserPrivilegeMapValue>();
            foreach (var userPrivilegeMap in user.UserPrivilegeMap)
            {
                if (userPrivilegeMap != null)
                {
                    var userPrivilegeMapValue = new UserDto.UserPrivilegeMapValue();
                    if (userPrivilegeMap.Privilege != null)
                    {
                        userPrivilegeMapValue.Privilege = new UserDto.PrivilegeValue()
                        {
                            PrivilegeId = userPrivilegeMap.Privilege.PrivilegeId,
                            PrivilegeName = userPrivilegeMap.Privilege.PrivilegeName
                        };
                    }
                    userDto.UserPrivilegeMap.Add(userPrivilegeMapValue);
                }
            }
            return userDto;
        }
    }
}
