using System.Linq;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Vse.AdminkaV1.DomAuthentication; // entity
using Vse.Routines.AspNetCore;
using Vse.Routines;

namespace Vse.AdminkaV1.Web.Controllers
{
    public class RolesController : Controller
    {
        const string BindedFields = nameof(Role.RoleId) + ", " + nameof(Role.RoleName);
        Include<Role> indexIncludes;
        Include<Role> detailsIncludes;
        Include<Role> editIncludes;
        Include<Role> deleteIncludes;
        public RolesController()
        {
            this.indexIncludes = includable =>
                includable.IncludeAll(y => y.RolesPrivileges)
                    .ThenInclude(y => y.Privilege)
                    .IncludeAll(y => y.UsersRoles)
                    .ThenInclude(y => y.User)
                    .IncludeAll(y => y.GroupsRoles)
                    .ThenInclude(y => y.Group);
            this.detailsIncludes = indexIncludes;
            this.editIncludes    = includable =>
                includable.IncludeAll(y => y.RolesPrivileges)
                    .ThenInclude(y => y.Privilege)
                    .IncludeAll(y => y.UsersRoles)
                    .ThenInclude(y => y.User)
                    .IncludeAll(y => y.GroupsRoles)
                    .ThenInclude(y => y.Group);
            this.deleteIncludes  = indexIncludes;
        }

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, Role>(
                (repository) =>
                {
                    var roles = repository.ToList(indexIncludes);
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
                var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolesPrivileges, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Rebase<Privilege>().ToList()
                    );
                var groupsNavigation = new MvcNavigationManager<Role, Group, GroupsRoles, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Rebase<Group>().ToList()
                );
                var usersNavigation = new MvcNavigationManager<Role, User, UsersRoles, int>(
                   this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                   repository.Rebase<User>().ToList()
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

               var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolesPrivileges, string>(
                   this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                   repository.Rebase<Privilege>().ToList()
               );
               privilegesNavigation.Parse(
                   e => new RolesPrivileges() { RoleId = entity.RoleId, PrivilegeId = e.PrivilegeId },
                   s => s
               );

               var groupsNavigation = new MvcNavigationManager<Role, Group, GroupsRoles, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Rebase<Group>().ToList()
               );
               groupsNavigation.Parse(
                    e => new GroupsRoles() { RoleId = entity.RoleId, GroupId = e.GroupId },
                    s => int.Parse(s)
               );

               var usersNavigation = new MvcNavigationManager<Role, User, UsersRoles, int>(
                    this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                    repository.Rebase<User>().ToList()
                );
               usersNavigation.Parse(
                    e => new UsersRoles() { RoleId = entity.RoleId, UserId = e.UserId },
                    s => int.Parse(s)
               );

               var mvcFork = new MvcFork(this, ModelState.IsValid);
               return mvcFork.Handle(
                   () => storage.Handle(
                       batch =>
                       {
                           batch.Add(entity);
                           batch.UpdateRelations(entity, e => e.RolesPrivileges, privilegesNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                           batch.UpdateRelations(entity, e => e.GroupsRoles, groupsNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
                           batch.UpdateRelations(entity, e => e.UsersRoles, usersNavigation.Selected, (e1, e2) => e1.RoleId == e2.RoleId);
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
                var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolesPrivileges, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Rebase<Privilege>().ToList()
                    );
                var groupsNavigation = new MvcNavigationManager<Role, Group, GroupsRoles, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Rebase<Group>().ToList()
                );
                var usersNavigation = new MvcNavigationManager<Role, User, UsersRoles, int>(
                   this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                   repository.Rebase<User>().ToList()
                );

                var mvcTube = new MvcTube(this);
                return mvcTube.Handle(
                        () => id != null,
                        () => repository.Find(e => e.RoleId == id, editIncludes),
                        (entity) =>
                        {
                            privilegesNavigation.Reset(entity.RolesPrivileges.Select(e => e.PrivilegeId));
                            groupsNavigation.Reset(entity.GroupsRoles.Select(e => e.RoleId));
                            usersNavigation.Reset(entity.UsersRoles.Select(e => e.UserId));
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

                var privilegesNavigation = new MvcNavigationManager<Role, Privilege, RolesPrivileges, string>(
                                   this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                                   repository.Rebase<Privilege>().ToList()
                               );
                privilegesNavigation.Parse(
                                    e => new RolesPrivileges() { RoleId = role.RoleId, PrivilegeId = e.PrivilegeId },
                                    s => s);

                var groupsNavigation = new MvcNavigationManager<Group, Group, GroupsRoles, int>(
                     this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                     repository.Rebase<Group>().ToList()
                 );
                groupsNavigation.Parse(
                                    e => new GroupsRoles() { RoleId = role.RoleId, GroupId = e.GroupId },
                                    s => int.Parse(s));

                var usersNavigation = new MvcNavigationManager<User, User, UsersRoles, int>(
                    this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                    repository.Rebase<User>().ToList()
                 );

                usersNavigation.Parse(
                                    e => new UsersRoles() { RoleId = role.RoleId, UserId = e.UserId },
                                    s => int.Parse(s));

                var mvcFork = new MvcFork(this, ModelState.IsValid);
                return mvcFork.Handle(
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Modify(role);
                            batch.UpdateRelations(role,
                                e => e.GroupsRoles,
                                groupsNavigation.Selected,
                                (e1, e2) => e1.RoleId == e2.RoleId
                            );
                            batch.UpdateRelations(role,
                                e => e.RolesPrivileges,
                                privilegesNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                            batch.UpdateRelations(role,
                                e => e.UsersRoles,
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
                        () => repository.Find(e => e.RoleId == id, deleteIncludes )
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
