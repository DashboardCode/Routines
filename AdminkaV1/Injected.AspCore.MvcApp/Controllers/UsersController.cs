using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Microsoft.Extensions.Configuration;

using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class UsersController : RoutineController
    {
        #region Meta
        static ControllerMeta<User, int> meta = new ControllerMeta<User, int>(
            id => e => e.UserId == id,
            Converters.TryParseInt,
            manyToMany => manyToMany
                .Add("Privileges",
                    repository => repository.Clone<Privilege>().List(),
                    nameof(Privilege.PrivilegeName),
                    e => e.UserPrivilegeMap,
                    mm => mm.PrivilegeId,
                    mm => mm.UserId,
                    e => e.PrivilegeId,
                    (ep, ef) => new UserPrivilege() { UserId = ep.UserId, PrivilegeId = ef.PrivilegeId }
                ).Add("Roles",
                    repository => repository.Clone<Role>().List(),
                    nameof(Role.RoleName),
                    e => e.UserRoleMap,
                    mm => mm.RoleId,
                    mm => mm.UserId,
                    e => e.RoleId,
                    (ep, ef) => new UserRole() { UserId = ep.UserId, RoleId = ef.RoleId }
                ).Add("Groups",
                    repository => repository.Clone<Group>().List(),
                    nameof(AuthenticationDom.User.LoginName),
                    e => e.UserGroupMap,
                    mm => mm.GroupId,
                    mm => mm.UserId,
                    e => e.GroupId,
                    (ep, ef) => new UserGroup() { GroupId = ep.UserId, UserId = ef.GroupId }
                ),
            chain => chain
                       .IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserGroupMap)
                       .ThenInclude(e => e.Group),
            chain => chain.IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserGroupMap)
                       .ThenInclude(e => e.Group),
            null,
            editables => 
                editables
                    .Add(e=>e.LoginName, Binder.ConvertToString)
                    .Add(e=>e.FirstName, Binder.ConvertToString)
                    .Add(e=>e.SecondName, Binder.ConvertToString),
            notEditables => notEditables.Add(e => e.UserId).Add(e=>e.RowVersion)
        );
        #endregion

        CrudRoutineControllerConsumer<User, int> consumer;
        public UsersController(IConfigurationRoot configurationRoot) :base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<User, int>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
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
            return await consumer.EditFormData();
        }
        #endregion
    }
}