using System;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler

using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using DashboardCode.Routines.Configuration.Standard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class UsersController : ConfigurableController
    {
        #region Meta
        static ControllerMeta<User, int> meta = new ControllerMeta<User, int>(
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
                editables.Include(e=>e.FirstName).Include(e=>e.SecondName).Include(e => e.LoginName),
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
        );
        #endregion

        Func<Task<IActionResult>> index;
        Func<Task<IActionResult>> details;
        Func<Task<IActionResult>> edit;
        Func<Task<IActionResult>> editConfirmed;
        public UsersController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) :base(applicationSettings, routineResolvablesOption.Value)
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index   = CrudRoutineControllerConsumer<User, int>.ComposeIndex(this, meta.IndexIncludes);
            details = CrudRoutineControllerConsumer<User, int>.ComposeDetails(
                this, meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
            edit = CrudRoutineControllerConsumer<User, int>.ComposeEdit(
                this, meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
            editConfirmed = CrudRoutineControllerConsumer<User, int>.ComposeEditConfirmed(
                this, authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, meta.ReferencesCollection.ParseRelated);
        }

        #region Details / Index
        public async Task<IActionResult> Details()
            => await details();

        public async Task<IActionResult> Index()
            => await index();
        #endregion

        #region Edit
        public async Task<IActionResult> Edit()
            => await edit();
 
        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFormData()
            => await editConfirmed();
        #endregion
    }
}