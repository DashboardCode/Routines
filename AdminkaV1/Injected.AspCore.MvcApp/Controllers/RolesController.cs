using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.Standard;

using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class RolesController : ConfigurableController
    {
        #region Meta
        static ControllerMeta<Role, int> meta = new ControllerMeta<Role, int>(
            id => e => e.RoleId == id,
            Converters.TryParseInt,
            chain => chain
                       .IncludeAll( e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll( e => e.GroupRoleMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll( e => e.UserRoleMap)
                       .ThenInclude(e => e.User),
            chain => chain
                       .IncludeAll( e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll( e => e.GroupRoleMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll( e => e.UserRoleMap)
                       .ThenInclude(e => e.User),
            null,
            editables =>
                editables
                    .Add(e=>e.RoleName, Binder.ConvertToString),
            notEditables => 
                notEditables
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
        );
        #endregion

        CrudRoutineControllerConsumer<Role, int> consumer;
        public RolesController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) : base(applicationSettings, routineResolvablesOption.Value)
        {
            consumer = new CrudRoutineControllerConsumer<Role, int>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public Task<IActionResult> Details()
        {
            return consumer.Details();
        }

        public Task<IActionResult> Index()
        {
            return consumer.Index();
        }
        #endregion

        #region Create
        public Task<IActionResult> Create()
        {
            return  consumer.Create();
        }

        [HttpPost, ActionName(nameof(Create)), ValidateAntiForgeryToken]
        public Task<IActionResult> CreateConfirmed()
        {
            return consumer.CreateConfirmed();
        }
        #endregion

        #region Edit
        public Task<IActionResult> Edit()
        {
            return consumer.Edit();
        }

        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public Task<IActionResult> EditFormData()
        {
            return consumer.EditConfirmed();
        }
        #endregion

        #region Delete
        public Task<IActionResult> Delete()
        {
            return consumer.Delete();
        }

        [HttpPost, ActionName(nameof(Delete)), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteFormData()
        {
            return consumer.DeleteConfirmed();
        }
        #endregion
     }
}