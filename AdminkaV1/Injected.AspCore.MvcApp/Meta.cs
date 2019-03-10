using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public static class Meta
    {
        public static GroupMeta GroupMeta = new GroupMeta();
        public static PrivilegeMeta PrivilegeMeta = new PrivilegeMeta();
        public static UserMeta UserMeta = new UserMeta();
        public static RoleMeta RoleMeta = new RoleMeta();
    }

    public class PrivilegeMeta : MvcMeta<Privilege, string>
    {
        public PrivilegeMeta() : base(
            id => e => e.PrivilegeId == id,
            Converters.TryParseString,
            chain => chain.IncludeAll(e => e.GroupPrivilegeMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.User),
            chain => chain.IncludeAll(e => e.GroupPrivilegeMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.User),
            chain => chain.Include(e => e.PrivilegeName),
            editables => editables.Add(e => e.PrivilegeName, StringValuesExtensions.ConvertToString),
            hiddenFormFields => hiddenFormFields.Add(e => e.PrivilegeId).Add(e => e.RowVersion),
            null,
            manyToMany => manyToMany
                .Add("Roles", "RolesMultiSelectList",
                     repository => repository.Clone<Role>().List(),
                     e => e.RolePrivilegeMap,
                     mm => mm.RoleId,
                     mm => mm.PrivilegeId,
                     e => e.RoleId,
                     nameof(Role.RoleId),
                     nameof(Role.RoleName),
                     (ep, ef) => new RolePrivilege() { PrivilegeId = ep.PrivilegeId, RoleId = ef.RoleId },
                     s => int.Parse(s)
                )
                .Add("Groups", "GroupsMultiSelectList",
                     repository => repository.Clone<Group>().List(),
                     e => e.GroupPrivilegeMap,
                     mm => mm.GroupId,
                     mm => mm.PrivilegeId,
                     e => e.GroupId,
                     nameof(Group.GroupId),
                     nameof(Group.GroupName),
                     (ep, ef) => new GroupPrivilege() { PrivilegeId = ep.PrivilegeId, GroupId = ef.GroupId }
                )
                .Add(
                    "Users", "UsersMultiSelectList",
                    repository => repository.Clone<User>().List(),
                    e => e.UserPrivilegeMap,
                    mm => mm.UserId,
                    mm => mm.PrivilegeId,
                    e => e.UserId,
                    nameof(AuthenticationDom.User.UserId),
                    nameof(AuthenticationDom.User.LoginName),
                    (ep, ef) => new UserPrivilege() { PrivilegeId = ep.PrivilegeId, UserId = ef.UserId }
                )
            )
        {

        }
    }

    public class GroupMeta : MvcMeta<Group, int>
    {
        public GroupMeta() : base(
            id => e => e.GroupId == id,
            Converters.TryParseInt,
            chain => chain
                .IncludeAll(e => e.GroupPrivilegeMap)
                .ThenInclude(e => e.Privilege)
                .IncludeAll(e => e.GroupRoleMap)
                .ThenInclude(e => e.Role)
                .IncludeAll(e => e.UserGroupMap)
                .ThenInclude(e => e.User),
            chain => chain.IncludeAll(e => e.GroupPrivilegeMap)
                .ThenInclude(e => e.Privilege)
                .IncludeAll(e => e.GroupRoleMap)
                .ThenInclude(e => e.Role)
                .IncludeAll(e => e.UserGroupMap)
                .ThenInclude(e => e.User),
            null,
            //var roleNameLength = metaBrowser.GetLength<Role>(en => en.RoleName);
            formFields =>
                formFields.Add(e => e.GroupName, StringValuesExtensions.ConvertToString)
                          .Add(e => e.GroupAdName, StringValuesExtensions.ConvertToString, asserts => asserts.Add(v => v.Length <= 126, "Too long!"))
            //.Add(e => e.GroupAdName, setter => sv => StringValuesExtensions.TryStringValidateLength(sv, v => setter(v), 100))
            //.Add(e => e.GroupId,  StringValuesExtensions.ConvertToInt, asserts => asserts.Add(v => v < 100, "Too long!"))
            //.Add(e => e.GroupId,  StringValuesExtensions.ConvertToInt, v=>new BinderResult(v<100? null: new[] { "Too long!" }), converter => setter => validator => validator(setter(converter())))
            //.Add(e => e.GroupId,  StringValuesExtensions.ConvertToInt,  convertor => setter => { var r = setter(convertor()); if (r.Value > 100) {; } return r.ToVerboseResult(); })
            //.Add(e => e.GroupId,  setter => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); setter(v);    return new BinderResult(v<100?null: "Too long!"); })
            //.Add("GroupId",           e => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); e.GroupId = v; return new BinderResult(v<100?null:"Too bug!"); })
            ,

            hiddenFormFields =>
                hiddenFormFields.Add(e => e.GroupId)
                            .Add(e => e.RowVersion)
            //.Add(e => e.GroupId, sv => int.Parse(sv.ToString()))
            //.Add(e => e.GroupId, sv => int.Parse(sv.ToString()), convertor => setter => setter(convertor()))
            //.Add(e => e.GroupId, setter => sv => setter(int.Parse(sv.ToString())))
            //.Add("GroupId", e => e.GroupId, setter => sv => setter(int.Parse(sv.ToString())))
            //.Add("GroupId", e => sv => e.GroupId = int.Parse(sv.ToString()))
            ,
            null,
            manyToMany => manyToMany.Add(
                "Privileges",
                "PrivilegesMultiSelectList",
                repository => repository.Clone<Privilege>().List(),
                e => e.GroupPrivilegeMap,
                mm => mm.PrivilegeId,
                mm => mm.GroupId,
                e => e.PrivilegeId,
                nameof(Privilege.PrivilegeId),
                nameof(Privilege.PrivilegeName),
                (ep, ef) => new GroupPrivilege() { GroupId = ep.GroupId, PrivilegeId = ef.PrivilegeId }
            ).Add(
                "Roles",
                "RolesMultiSelectList",
                repository => repository.Clone<Role>().List(),
                e => e.GroupRoleMap,
                mm => mm.RoleId,
                mm => mm.GroupId,
                e => e.RoleId,
                nameof(Role.RoleId),
                nameof(Role.RoleName),
                (ep, ef) => new GroupRole() { GroupId = ep.GroupId, RoleId = ef.RoleId }
            ).Add(
                "Users",
                "UsersMultiSelectList",
                repository => repository.Clone<User>().List(),
                e => e.UserGroupMap,
                mm => mm.UserId,
                mm => mm.GroupId,
                e => e.UserId,
                nameof(AuthenticationDom.User.UserId),
                nameof(AuthenticationDom.User.LoginName),
                (ep, ef) => new UserGroup() { GroupId = ep.GroupId, UserId = ef.UserId }
            )

            )
        {

        }
    }

    public class UserMeta : MvcMeta<User, int>
    {
        public UserMeta() : base(
            findByIdExpression: id => e => e.UserId == id,
            keyConverter: Converters.TryParseInt,
            indexIncludes: chain => chain
                       .IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserGroupMap)
                       .ThenInclude(e => e.Group),
            editIncludes: chain => chain.IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserGroupMap)
                       .ThenInclude(e => e.Group),
            disabledProperties: editables =>
                editables.Include(e => e.FirstName).Include(e => e.SecondName).Include(e => e.LoginName),
            addEditableBinders: null,
            addNotEditableBinders: notEditables =>
                notEditables
                    .Add(e => e.UserId)
                    .Add(e => e.RowVersion),
            oneToMany: null,
            manyToMany: manyToMany => manyToMany
                .Add("Privileges", "PrivilegesMultiSelectList",
                    repository => repository.Clone<Privilege>().List(),
                    e => e.UserPrivilegeMap,
                    mm => mm.PrivilegeId,
                    mm => mm.UserId,
                    e => e.PrivilegeId,
                    nameof(Privilege.PrivilegeId),
                    nameof(Privilege.PrivilegeName),
                    (ep, ef) => new UserPrivilege() { UserId = ep.UserId, PrivilegeId = ef.PrivilegeId }
                ).Add("Roles", "RolesMultiSelectList",
                    repository => repository.Clone<Role>().List(),
                    e => e.UserRoleMap,
                    mm => mm.RoleId,
                    mm => mm.UserId,
                    e => e.RoleId,
                    nameof(Role.RoleId),
                    nameof(Role.RoleName),
                    (ep, ef) => new UserRole() { UserId = ep.UserId, RoleId = ef.RoleId }
                ).Add("Groups", "GroupsMultiSelectList",
                    repository => repository.Clone<Group>().List(),
                    e => e.UserGroupMap,
                    mm => mm.GroupId,
                    mm => mm.UserId,
                    e => e.GroupId,
                    nameof(Group.GroupId),
                    nameof(Group.GroupName),
                    (ep, ef) => new UserGroup() { GroupId = ep.UserId, UserId = ef.GroupId }
                )
            )
        {

        }
    }

    public class RoleMeta : MvcMeta<Role, int>
    {
        public RoleMeta() : base(
            id => e => e.RoleId == id,
            Converters.TryParseInt,
            chain => chain
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.GroupRoleMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.User),
            chain => chain
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.GroupRoleMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.User),
            null,
            editables =>
                editables
                    .Add(e => e.RoleName, StringValuesExtensions.ConvertToString),
            hiddenFormFields =>
                hiddenFormFields
                    .Add(e => e.RoleId)
                    .Add(e => e.RowVersion),
            null,
            manyToMany => manyToMany
                .Add("Privileges", "PrivilegesMultiSelectList",
                    repository => repository.Clone<Privilege>().List(),
                    e => e.RolePrivilegeMap,
                    e => e.PrivilegeId,
                    mm => mm.RoleId,
                    e => e.PrivilegeId,
                    nameof(Privilege.PrivilegeId),
                    nameof(Privilege.PrivilegeName),
                    (ep, ef) => new RolePrivilege() { RoleId = ep.RoleId, PrivilegeId = ef.PrivilegeId }
                 ).Add(
                    "Groups", "GroupsMultiSelectList",
                    repository => repository.Clone<Group>().List(),
                    e => e.GroupRoleMap,
                    mm => mm.GroupId,
                    mm => mm.RoleId,
                    e => e.GroupId,
                    nameof(Group.GroupId),
                    nameof(Group.GroupName),
                    (ep, ef) => new GroupRole() { RoleId = ep.RoleId, GroupId = ef.GroupId }
                 ).Add(
                    "Users", "UsersMultiSelectList",
                    repository => repository.Clone<User>().List(),
                    e => e.UserRoleMap,
                    mm => mm.UserId,
                    mm => mm.RoleId,
                    e => e.UserId,
                    nameof(AuthenticationDom.User.UserId),
                    nameof(AuthenticationDom.User.LoginName),
                    (ep, ef) => new UserRole() { RoleId = ep.RoleId, UserId = ef.UserId }
                )
            )
        {

        }
    }
}
