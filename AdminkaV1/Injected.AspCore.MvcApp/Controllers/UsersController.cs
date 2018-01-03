using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Microsoft.Extensions.Configuration;

using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;
using System;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class UsersController : ConfigurableController
    {
        #region Meta
        static ControllerMeta<User, int> meta = new ControllerMeta<User, int>(
            id => e => e.UserId == id,
            Converters.TryParseInt,
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
                    .Add(e=>e.LoginName,  Binder.ConvertToString)
                    .Add(e=>e.FirstName,  Binder.ConvertToString)
                    .Add(e=>e.SecondName, Binder.ConvertToString),
            notEditables => 
                notEditables
                    .Add(e => e.UserId)
                    .Add(e => e.RowVersion),
            null,
            manyToMany => manyToMany
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
        );
        #endregion

        CrudRoutineControllerConsumer<User, int> consumer;
        Func<Task<IActionResult>> index;
        Func<Task<IActionResult>> details;
        public UsersController(IConfigurationRoot configurationRoot) :base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<User, int>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
            this.index   = CrudRoutineControllerConsumer<User, int>.ComposeIndex(this, meta.IndexIncludes);
            //this.details = consumer.ComposeDetails();
        }

        #region Details / Index
        public async Task<IActionResult> Details()
        {
            return await details();
        }

        public async Task<IActionResult> Index()
        {
            return await index();
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