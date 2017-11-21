using System.Linq;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using Microsoft.Extensions.Configuration;
using DashboardCode.Routines;
using System;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class RolesController : RoutineController
    {
        const string BindedFields = nameof(Role.RoleId) + ", " + nameof(Role.RoleName);
        Include<Role> indexIncludes;
        Include<Role> detailsIncludes;
        Include<Role> editIncludes;
        Include<Role> deleteIncludes;
        public RolesController(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
            this.indexIncludes = includable =>
                includable.IncludeAll(y => y.RolePrivilegeMap)
                    .ThenInclude(y => y.Privilege)
                    .IncludeAll(y => y.UserRoleMap)
                    .ThenInclude(y => y.User)
                    .IncludeAll(y => y.GroupRoleMap)
                    .ThenInclude(y => y.Group);
            this.detailsIncludes = indexIncludes;
            this.editIncludes = includable =>
             includable.IncludeAll(y => y.RolePrivilegeMap)
                 .ThenInclude(y => y.Privilege)
                 .IncludeAll(y => y.UserRoleMap)
                 .ThenInclude(y => y.User)
                 .IncludeAll(y => y.GroupRoleMap)
                 .ThenInclude(y => y.Group);
            this.deleteIncludes = indexIncludes;
        }

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, Role>(
                (repository) =>
                {
                    var roles = repository.List(indexIncludes);
                    return View(roles);
                });
        }

        public async Task<IActionResult> Details(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Role>(repository =>
            {
                return this.MakeActionResultOnRequest(
                    () => id != null,
                    () => repository.Find(e => e.RoleId == id, detailsIncludes)
                );
            });
        }

        public async Task<IActionResult> Create()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, Role>(repository =>
            {
                var privilegesNavigation = new MvcNavigationFacade<Role, Privilege, RolePrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Clone<Privilege>().List()
                    );
                var groupsNavigation = new MvcNavigationFacade<Role, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Clone<Group>().List()
                );
                var usersNavigation = new MvcNavigationFacade<Role, User, UserRole, int>(
                   this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
                   repository.Clone<User>().List()
                );
                privilegesNavigation.Reset();
                groupsNavigation.Reset();
                usersNavigation.Reset();
                return View();
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(BindedFields)] Role entity)
        {
            var routine = new MvcRoutine(this, new { role = entity });
            return await routine.HandleStorageAsync<IActionResult, Role>((repository, storage, state) =>
           {
               if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                   return Unauthorized();

               var privilegesNavigation = new MvcNavigationFacade<Role, Privilege, RolePrivilege, string>(
                   this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                   repository.Clone<Privilege>().List()
               );
               privilegesNavigation.Parse(
                   e => new RolePrivilege() { RoleId = entity.RoleId, PrivilegeId = e.PrivilegeId },
                   s => s
               );

               var groupsNavigation = new MvcNavigationFacade<Role, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Clone<Group>().List()
               );
               groupsNavigation.Parse(
                    e => new GroupRole() { RoleId = entity.RoleId, GroupId = e.GroupId },
                    s => int.Parse(s)
               );

               var usersNavigation = new MvcNavigationFacade<Role, User, UserRole, int>(
                    this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
                    repository.Clone<User>().List()
                );
               usersNavigation.Parse(
                    e => new UserRole() { RoleId = entity.RoleId, UserId = e.UserId },
                    s => int.Parse(s)
               );

               return this.MakeActionResultOnSave(
                   ModelState.IsValid,
                   () => storage.Handle(
                       batch =>
                       {
                           batch.Add(entity);
                           batch.ModifyWithRelated(entity, e => e.RolePrivilegeMap, privilegesNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                           batch.ModifyWithRelated(entity, e => e.GroupRoleMap, groupsNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                           batch.ModifyWithRelated(entity, e => e.UserRoleMap, usersNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                       }),
                   () =>
                   {
                       privilegesNavigation.Reset();
                       groupsNavigation.Reset();
                       usersNavigation.Reset();
                       return View(entity);
                   }
               );
           });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Role>(repository =>
            {
                var privilegesNavigation = new MvcNavigationFacade<Role, Privilege, RolePrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Clone<Privilege>().List()
                    );
                var groupsNavigation = new MvcNavigationFacade<Role, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Clone<Group>().List()
                );
                var usersNavigation = new MvcNavigationFacade<Role, User, UserRole, int>(
                   this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
                   repository.Clone<User>().List()
                );

                return this.MakeActionResultOnRequest(
                        () => id != null,
                        () => repository.Find(e => e.RoleId == id, editIncludes),
                        (entity) =>
                        {
                            privilegesNavigation.Reset(entity.RolePrivilegeMap.Select(e => e.PrivilegeId));
                            groupsNavigation.Reset(entity.GroupRoleMap.Select(e => e.RoleId));
                            usersNavigation.Reset(entity.UserRoleMap.Select(e => e.UserId));
                        }
                    );
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(BindedFields)] Role role)
        {
            var routine = new MvcRoutine(this, new { role = role });
            return await routine.HandleTransactionAsync<IActionResult, Role>((commit, state) =>
            {
                    if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                        return Unauthorized();

                    return commit(
                        (repository, save) =>
                        {
                            #region Prepare navigation facades
                                var privilegesNavigation = new MvcNavigationFacade<Role, Privilege, RolePrivilege, string>(
                                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                                    repository.Clone<Privilege>().List()
                                );
                                privilegesNavigation.Parse(
                                    e => new RolePrivilege() { RoleId = role.RoleId, PrivilegeId = e.PrivilegeId },
                                    s => s);

                                var groupsNavigation = new MvcNavigationFacade<Group, Group, GroupRole, int>(
                                     this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                                     repository.Clone<Group>().List()
                                );
                                groupsNavigation.Parse(
                                     e => new GroupRole() { RoleId = role.RoleId, GroupId = e.GroupId },
                                     s => int.Parse(s));

                                var usersNavigation = new MvcNavigationFacade<User, User, UserRole, int>(
                                     this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
                                     repository.Clone<User>().List()
                                );
                                usersNavigation.Parse(
                                     e => new UserRole() { RoleId = role.RoleId, UserId = e.UserId },
                                     s => int.Parse(s));
                            #endregion

                            return this.MakeActionResultOnSave(
                                 () =>
                                     save(
                                        batch =>
                                        {
                                            batch.Modify(role);
                                            batch.ModifyWithRelated(role,
                                                 e => e.GroupRoleMap,
                                                 groupsNavigation.Selected,
                                                 (e1, e2) => e1.RoleId == e2.RoleId
                                            );
                                            batch.ModifyWithRelated(role,
                                                 e => e.RolePrivilegeMap,
                                                 privilegesNavigation.Selected,
                                                 (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                                            );
                                            batch.ModifyWithRelated(role,
                                                 e => e.UserRoleMap,
                                                 usersNavigation.Selected,
                                                 (e1, e2) => e1.RoleId == e2.RoleId
                                            );
                                        }),
                                 () =>
                                    {
                                        privilegesNavigation.Reset();
                                        groupsNavigation.Reset();
                                        usersNavigation.Reset();
                                        return View(role);
                                    }
                            );
                        }
                    );
                });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Role>(repository =>
            {
                return this.MakeActionResultOnRequest(
                        () => id != null,
                        () => repository.Find(e => e.RoleId == id, deleteIncludes)
                    );
            });
        }
        [HttpPost, ActionName(nameof(RolesController.Delete)), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleTransactionAsync<IActionResult, Role>((commit, state) =>
            {
                if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                    return Unauthorized();
                return commit( (repository, save) =>
                    {
                        var entity = repository.Find(e => e.RoleId == id);
                        return this.MakeActionResultOnSave(
                                true,
                                () => save(batch => batch.Remove(entity)),
                                () => View(nameof(RolesController.Delete), entity)
                            );
                    });
            });
        }
    }
}