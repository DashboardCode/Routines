using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class GroupsController : RoutineController
    {
        #region Meta
        static ControllerMeta<Group, int?> meta = new ControllerMeta<Group, int?>(
            () => new Group(),
            new ReferencesMeta<Group>(new IManyToMany<Group>[] {
                new ManyToMany<Group, Privilege, GroupPrivilege, string>(
                    new MvcNavigationFacade2<Group, Privilege, GroupPrivilege, string>(
                        "Privileges", e => e.PrivilegeId,
                        nameof(Privilege.PrivilegeName),
                        (ep, ef) => new GroupPrivilege() { GroupId = ep.GroupId, PrivilegeId = ef.PrivilegeId },
                        s => s
                    ),
                    repository=>repository.Clone<Privilege>().List(),
                    e => e.GroupPrivilegeMap,
                    (e1, e2) => e1.PrivilegeId == e2.PrivilegeId,
                    e => e.PrivilegeId
                ),
                new ManyToMany<Group, Role, GroupRole, int>(
                    new MvcNavigationFacade2<Group, Role, GroupRole, int>(
                        "Roles", e => e.RoleId,
                        nameof(Role.RoleName),
                        (ep, ef) => new GroupRole() { GroupId = ep.GroupId, RoleId = ef.RoleId },
                        s => int.Parse(s)
                    ),
                    repository=>repository.Clone<Role>().List(),
                    e => e.GroupRoleMap,
                    (e1, e2) => e1.RoleId == e2.RoleId,
                    e => e.RoleId
                ),
                new ManyToMany<Group, User, UserGroup, int>(
                    new MvcNavigationFacade2<Group, User, UserGroup, int>(
                        "Users", e => e.UserId,
                        nameof(AuthenticationDom.User.LoginName),
                        (ep, ef) => new UserGroup() { GroupId = ep.GroupId, UserId = ef.UserId },
                        s => int.Parse(s)
                    ),
                    repository=>repository.Clone<User>().List(),
                    e => e.UserGroupMap,
                    (e1, e2) => e1.UserId == e2.UserId,
                    e => e.UserId
                )
            }),
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
            id => e => e.GroupId == id.Value,
            null,
            //var roleNameLength = metaBrowser.GetLength<Role>(en => en.RoleName);
            //editables.Add(e => e.GroupName, sv => Binder.ConvertToString(sv), s=>Binder.ValidateStringLength(10) )
            editables =>
                editables.Add(e => e.GroupName, Binder.ConvertToString).Add(e => e.GroupAdName, Binder.ConvertToString, asserts => asserts.Add(v => v.Length <= 6, "Too big!"))

                         //.Add(e => e.GroupAdName, setter => sv => Binder.TryStringValidateLength(sv, v => setter(v), 100))
                         //.Add(e => e.GroupId,  Binder.ConvertToInt, asserts => asserts.Add(v => v < 100, "Too big!"))
                         //.Add(e => e.GroupId,  Binder.ConvertToInt, v=>new BinderResult(v<100? null: "Too big!"), converter => setter => validator => validator(setter(converter())))
                         //.Add(e => e.GroupId,  Binder.ConvertToInt,  convertor => setter => { var r = setter(convertor()); if (r.Value > 100) {; } return r.BinderResult; })
                         //.Add(e => e.GroupId,  setter => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); setter(v);     return new BinderResult(v<100?null: "Too big!"); })
                         //.Add("GroupId",           e => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); e.GroupId = v; return new BinderResult(v<100?null:"Too bug!"); })
            ,

            notEditables =>
                notEditables.Add(e => e.GroupId).Add(e => e.RowVersion)
            //.Add(e => e.GroupId, sv => int.Parse(sv.ToString()))
            //.Add(e => e.GroupId, sv => int.Parse(sv.ToString()), convertor => setter => setter(convertor()))
            //.Add(e => e.GroupId, setter => sv => setter(int.Parse(sv.ToString())))
            //.Add("GroupId", e => e.GroupId, setter => sv => setter(int.Parse(sv.ToString())))
            //.Add("GroupId", e => sv => e.GroupId = int.Parse(sv.ToString()))


        //
        );
        #endregion

        CrudRoutineControllerConsumer<Group, int?> consumer;
        public GroupsController(IConfigurationRoot  configurationRoot) :base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<Group, int?>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public async Task<IActionResult> Details(int? id)
        {
            return await consumer.Details(id);
        }

        public async Task<IActionResult> Index()
        {
            return await consumer.Index();
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            return await consumer.Create();
        }

        [HttpPost, ActionName(nameof(Create)), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed()
        {
            return await consumer.CreateConfirmed();
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            return await consumer.Edit(id);
        }

        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed()
        {
            return await consumer.EditConfirmed();
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int? id)
        {
            return await consumer.Delete(id);
        }

        [HttpPost, ActionName(nameof(Delete)), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            return await consumer.DeleteConfirmed(id);
        }
        #endregion
    }
}