using System.Linq;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using DashboardCode.AdminkaV1.DomAuthentication; // entity
using DashboardCode.Routines.AspNetCore;
using Microsoft.Extensions.Configuration;
using DashboardCode.Routines;

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
                var mvcTube = new MvcTube(this);
                return mvcTube.Handle(
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
                var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolePrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Sprout<Privilege>().List()
                    );
                var groupsNavigation = new MvcNavigationManager<Role, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Sprout<Group>().List()
                );
                var usersNavigation = new MvcNavigationManager<Role, User, UserRole, int>(
                   this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                   repository.Sprout<User>().List()
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

               var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolePrivilege, string>(
                   this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                   repository.Sprout<Privilege>().List()
               );
               privilegesNavigation.Parse(
                   e => new RolePrivilege() { RoleId = entity.RoleId, PrivilegeId = e.PrivilegeId },
                   s => s
               );

               var groupsNavigation = new MvcNavigationManager<Role, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Sprout<Group>().List()
               );
               groupsNavigation.Parse(
                    e => new GroupRole() { RoleId = entity.RoleId, GroupId = e.GroupId },
                    s => int.Parse(s)
               );

               var usersNavigation = new MvcNavigationManager<Role, User, UserRole, int>(
                    this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                    repository.Sprout<User>().List()
                );
               usersNavigation.Parse(
                    e => new UserRole() { RoleId = entity.RoleId, UserId = e.UserId },
                    s => int.Parse(s)
               );

               var mvcFork = new MvcFork(this, ModelState.IsValid);
               return mvcFork.Handle(
                   () => storage.Handle(
                       batch =>
                       {
                           batch.Add(entity);
                           batch.UpdateRelations(entity, e => e.RolePrivilegeMap, privilegesNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                           batch.UpdateRelations(entity, e => e.GroupRoleMap, groupsNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                           batch.UpdateRelations(entity, e => e.UserRoleMap, usersNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
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
                var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolePrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Sprout<Privilege>().List()
                    );
                var groupsNavigation = new MvcNavigationManager<Role, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Sprout<Group>().List()
                );
                var usersNavigation = new MvcNavigationManager<Role, User, UserRole, int>(
                   this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                   repository.Sprout<User>().List()
                );

                var mvcTube = new MvcTube(this);
                return mvcTube.Handle(
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
            return await routine.HandleStorageAsync<IActionResult, Role>(
                (repository, storage, state) =>
            {
                if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                    return Unauthorized();

                var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolePrivilege, string>(
                                   this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                                   repository.Sprout<Privilege>().List()
                               );
                privilegesNavigation.Parse(
                                    e => new RolePrivilege() { RoleId = role.RoleId, PrivilegeId = e.PrivilegeId },
                                    s => s);

                var groupsNavigation = new MvcNavigationManager<Group, Group, GroupRole, int>(
                     this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                     repository.Sprout<Group>().List()
                 );
                groupsNavigation.Parse(
                                    e => new GroupRole() { RoleId = role.RoleId, GroupId = e.GroupId },
                                    s => int.Parse(s));

                var usersNavigation = new MvcNavigationManager<User, User, UserRole, int>(
                    this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                    repository.Sprout<User>().List()
                 );

                usersNavigation.Parse(
                                    e => new UserRole() { RoleId = role.RoleId, UserId = e.UserId },
                                    s => int.Parse(s));

                var mvcFork = new MvcFork(this, ModelState.IsValid);
                return mvcFork.Handle(
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Modify(role);
                            batch.UpdateRelations(role,
                                e => e.GroupRoleMap,
                                groupsNavigation.Selected,
                                (e1, e2) => e1.RoleId == e2.RoleId
                            );
                            batch.UpdateRelations(role,
                                e => e.RolePrivilegeMap,
                                privilegesNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                            batch.UpdateRelations(role,
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
            });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Role>(repository =>
            {
                var mvcTube = new MvcTube(this);
                return mvcTube.Handle(
                        () => id != null,
                        () => repository.Find(e => e.RoleId == id, deleteIncludes)
                    );
            });
        }
        [HttpPost, ActionName(nameof(RolesController.Delete)), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Role>((repository, storage, state) =>
            {
                if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                    return Unauthorized();
                var entity = repository.Find(e => e.RoleId == id);
                var mvcFork = new MvcFork(this);
                return mvcFork.Handle(
                        () => storage.Handle(batch => batch.Remove(entity)),
                        () => View(nameof(RolesController.Delete), entity)
                    );
            });
        }
    }
}
