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
            () => new User(),
            id => e => e.UserId == id,
            Converters.TryParseInt,
            manyToMany => manyToMany
                .Add("Privileges",
                    repository => repository.Clone<Privilege>().List(),
                    e => e.UserPrivilegeMap,
                    e => e.PrivilegeId,
                    new MvcNavigationFacade<User, Privilege, UserPrivilege, string>(
                        e => e.PrivilegeId,
                        nameof(Privilege.PrivilegeName),
                        (ep, ef) => new UserPrivilege() { UserId = ep.UserId, PrivilegeId = ef.PrivilegeId }
                    )
                ).Add("Roles",
                                    repository => repository.Clone<Role>().List(),
                    e => e.UserRoleMap,
                    e => e.RoleId,
                     new MvcNavigationFacade<User, Role, UserRole, int>(
                         e => e.RoleId,
                        nameof(Role.RoleName),
                        (ep, ef) => new UserRole() { UserId = ep.UserId, RoleId = ef.RoleId }
                    )
                ).Add("Groups",
                    repository => repository.Clone<Group>().List(),
                    e => e.UserGroupMap,
                    e => e.UserId,
                    new MvcNavigationFacade<User, Group, UserGroup, int>(
                         e => e.GroupId,
                        nameof(AuthenticationDom.User.LoginName),
                        (ep, ef) => new UserGroup() { GroupId = ep.UserId, UserId = ef.GroupId }
                    )
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
                editables.Add(e=>e.LoginName, Binder.ConvertToString)
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