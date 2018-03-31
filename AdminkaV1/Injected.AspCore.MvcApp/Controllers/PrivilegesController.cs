using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.NETStandard;

using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class PrivilegesController : ConfigurableController
    {
        #region Meta
        static ControllerMeta<Privilege, string> meta = new ControllerMeta<Privilege, string>(
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
            editables => editables.Add(e=>e.PrivilegeName,  Binder.ConvertToString),
            notEditables => notEditables.Add(e => e.PrivilegeId).Add(e => e.RowVersion),
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
        );
        #endregion

        CrudRoutineControllerConsumer<Privilege, string> consumer;
        public PrivilegesController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) :base(applicationSettings, routineResolvablesOption.Value)
        {
            consumer = new CrudRoutineControllerConsumer<Privilege, string>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public async Task<IActionResult> Details()
        {
            return await consumer.Details();
        }

        public async Task<IActionResult> Index()
        {
            return await consumer.Index();
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit()
        {
            return await consumer.Edit();
        }

        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFormData()
        {
            return await consumer.EditConfirmed();
        }
        #endregion
    }
}
