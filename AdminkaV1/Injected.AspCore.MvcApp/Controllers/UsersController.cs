using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Microsoft.Extensions.Configuration;

using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class UsersController : RoutineController
    {
        #region Meta
        static ControllerMeta<User, int?> meta = new ControllerMeta<User, int?>(
            () => new User(),
            new ReferencesMeta<User>(new IManyToMany<User>[] {
                new ManyToMany<User, Privilege, UserPrivilege, string>(
                    new MvcNavigationFacade2<User, Privilege, UserPrivilege, string>(
                        "Privileges", e => e.PrivilegeId,
                        nameof(Privilege.PrivilegeName),
                        (ep, ef) => new UserPrivilege() { UserId = ep.UserId, PrivilegeId = ef.PrivilegeId },
                        s => s
                    ),
                    repository=>repository.Clone<Privilege>().List(),
                    e => e.UserPrivilegeMap,
                    (e1, e2) => e1.PrivilegeId == e2.PrivilegeId,
                    e => e.PrivilegeId
                ),
                new ManyToMany<User, Role, UserRole, int>(
                    new MvcNavigationFacade2<User, Role, UserRole, int>(
                        "Roles", e => e.RoleId,
                        nameof(Role.RoleName),
                        (ep, ef) => new UserRole() { UserId = ep.UserId, RoleId = ef.RoleId },
                        s => int.Parse(s)
                    ),
                    repository=>repository.Clone<Role>().List(),
                    e => e.UserRoleMap,
                    (e1, e2) => e1.RoleId == e2.RoleId,
                    e => e.RoleId
                ),
                new ManyToMany<User, Group, UserGroup, int>(
                    new MvcNavigationFacade2<User, Group, UserGroup, int>(
                        "Groups", e => e.GroupId,
                        nameof(AuthenticationDom.User.LoginName),
                        (ep, ef) => new UserGroup() { GroupId = ep.UserId, UserId = ef.GroupId },
                        s => int.Parse(s)
                    ),
                    repository=>repository.Clone<Group>().List(),
                    e => e.UserGroupMap,
                    (e1, e2) => e1.UserId == e2.UserId,
                    e => e.UserId
                )
            }),
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
            id => e => e.UserId == id.Value,
            null,
            editables => 
                editables.Add(e=>e.LoginName, setter=> e => sv => Binder.TryStringValidateLength(sv, v=> setter(e, v), 100))
                    .Add(e=>e.FirstName, setter => e => sv => Binder.TryStringValidateLength(sv, v => setter(e, v), 100))
                    .Add(e=>e.SecondName, setter => e => sv => Binder.TryStringValidateLength(sv, v => setter(e,v), 100)),
            notEditables => notEditables.Add(e => e.UserId).Add(e=>e.RowVersion)
            
        );
        #endregion

        CrudRoutineControllerConsumer<User, int?> consumer;
        public UsersController(IConfigurationRoot configurationRoot) :base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<User, int?>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
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
    }
}