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
            editables =>
                editables.Add(e => e.GroupName,   setter => e => sv => Binder.TryString(sv, v => setter(e, v)))
                         .Add(e => e.GroupAdName, setter => e => sv => Binder.TryStringValidateLength(sv, v => setter(e,v), 100))
                         //editables..Add(e => e.GroupName,   setter => e => sv => Binder.TryString(sv, v => setter(e, v)))
                         //editables.Add("GroupName", e => sv => Binder.TryString(sv, v => e.GroupName = v));
                         //editables.Add("GroupName",   e => Binder.Try(sv => new ConvertResult<string> { Value = sv.ToString() }, v => { e.GroupName = v; return new BinderResult(); }));
                         //editables.Add(e => e.GroupName,   setter => e => Binder.Try(sv => new ConvertResult<string> { Value = sv.ToString() }, v => { setter(e,v); return new BinderResult(); }))
                         
                         //editables.Add(e => e.GroupAdName, setter => e => sv => Binder.TryStringValidateLength(sv, v => setter(e,v), 100))
                         //editables.Add("GroupAdName", e => sv => Binder.TryStringValidateLength(sv, v => e.GroupAdName = v, 100));
                         //editables.Add("UserId", e => e.UserId,       setter => e => Binder.Try(sv=> new ConvertResult<int>{Value = int.Parse(sv.ToString())}, v => setter(e, v)));

            ,
            notEditables => 
                notEditables.Add(e=> e.GroupId).Add(e => e.RowVersion)
                //notEditables.Add(e => e.GroupId, sv => int.Parse(sv.ToString()));
                //notEditables.Add("GroupId", e => e.GroupId, setter => e => sv => setter(e, int.Parse(sv.ToString())));
                //notEditables.Add("GroupId",    e => sv => e.GroupId= int.Parse(sv.ToString()));
                //notEditables.Add("RowVersion", e => sv => e.RowVersion = Convert.FromBase64String(sv.ToString()));
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