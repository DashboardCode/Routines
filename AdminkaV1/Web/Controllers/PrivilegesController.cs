using System.Linq;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Vse.AdminkaV1.DomAuthentication; // entity
using Vse.Routines.AspNetCore;
using Vse.Routines;

namespace Vse.AdminkaV1.Web.Controllers
{
    public class PrivilegesController : Controller
    {
        const string BindedFields = nameof(Privilege.PrivilegeId) + ", " + nameof(Privilege.PrivilegeName);
        Include<Privilege> indexIncludes;
        Include<Privilege> detailsIncludes;
        Include<Privilege> editIncludes;
        public PrivilegesController()
        {
            this.indexIncludes = includable =>
                includable.IncludeAll(y => y.RolesPrivileges)
                    .ThenInclude(y => y.Role)
                    .IncludeAll(y => y.UsersPrivileges)
                    .ThenInclude(y => y.User)
                    .IncludeAll(y => y.GroupsPrivileges)
                    .ThenInclude(y => y.Group);
            this.detailsIncludes = indexIncludes;
            this.editIncludes = indexIncludes;
        }

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, Privilege>(
                (repository) =>
                {
                    var entities = repository.ToList(indexIncludes);
                    return View(entities);
                });
        }

        public async Task<IActionResult> Details(string id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Privilege>(repository =>
            {
                var mvcTube = new MvcTube(this);
                return mvcTube.Handle(
                    () => id != null,
                    () => repository.Find(e => e.PrivilegeId == id, detailsIncludes)
                );
            });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Privilege>(repository =>
            {
                var rolesNavigation = new MvcNavigationManager<Privilege, Role, RolesPrivileges, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Rebase<Role>().ToList()
                    );
                var groupsNavigation = new MvcNavigationManager<Privilege, Group, GroupsRoles, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Rebase<Group>().ToList()
                );
                var usersNavigation = new MvcNavigationManager<Privilege, User, UsersRoles, int>(
                   this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                   repository.Rebase<User>().ToList()
                );

                var mvcTube = new MvcTube(this);
                return mvcTube.Handle(
                        () => id != null,
                        () => repository.Find(e => e.PrivilegeId == id, editIncludes),
                        (entity) =>
                        {
                            rolesNavigation.Reset(entity.RolesPrivileges.Select(e => e.RoleId));
                            groupsNavigation.Reset(entity.GroupsPrivileges.Select(e => e.GroupId));
                            usersNavigation.Reset(entity.UsersPrivileges.Select(e => e.UserId));
                        }
                    );
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(BindedFields)] Privilege entity)
        {
            var routine = new MvcRoutine(this, new { entity = entity });
            return await routine.HandleStorageAsync<IActionResult, Privilege>(
                (repository, storage, state) =>
            {
                if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                    return Unauthorized();

                var rolesNavigation = new MvcNavigationManager<Privilege, Role, RolesPrivileges, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Rebase<Role>().ToList()
                );
                rolesNavigation.Parse(
                    e => new RolesPrivileges() { PrivilegeId = entity.PrivilegeId, RoleId = e.RoleId },
                    s => int.Parse(s));

                var groupsNavigation = new MvcNavigationManager<Privilege, Group, GroupsPrivileges, int>(
                     this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                     repository.Rebase<Group>().ToList()
                 );
                groupsNavigation.Parse(
                                    e => new GroupsPrivileges() { PrivilegeId = entity.PrivilegeId, GroupId = e.GroupId },
                                    s => int.Parse(s));

                var usersNavigation = new MvcNavigationManager<Privilege, User, UsersPrivileges, int>(
                    this, "Users", e => e.UserId, nameof(DomAuthentication.User.LoginName),
                    repository.Rebase<User>().ToList()
                 );

                usersNavigation.Parse(
                                    e => new UsersPrivileges() { PrivilegeId = entity.PrivilegeId, UserId = e.UserId },
                                    s => int.Parse(s));

                var mvcFork = new MvcFork(this, ModelState.IsValid);
                return mvcFork.Handle(
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Modify(entity);
                            batch.UpdateRelations(entity,
                                e => e.GroupsPrivileges,
                                groupsNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                            batch.UpdateRelations(entity,
                                e => e.RolesPrivileges,
                                rolesNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                            batch.UpdateRelations(entity,
                                e => e.UsersPrivileges,
                                usersNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                        }),
                    () =>
                    {
                        rolesNavigation.Reset();
                        groupsNavigation.Reset();
                        usersNavigation.Reset();
                        return View(entity);
                    }
                );
            });
        }
    }
}
