using System.Threading.Tasks; // assync actions
using System.Linq;
using Microsoft.AspNetCore.Mvc; // controler
using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class UsersController : RoutineController
    {
        const string BindedFields = nameof(AuthenticationDom.User.UserId) + ", "
            + nameof(AuthenticationDom.User.LoginName) + ", "
            + nameof(AuthenticationDom.User.FirstName) + ", "
            + nameof(AuthenticationDom.User.SecondName);
        Include<User> indexIncludes;
        Include<User> detailsIncludes;
        Include<User> editIncludes;
        public UsersController(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
            this.indexIncludes = includable =>
                includable.IncludeAll(y => y.UserPrivilegeMap)
                    .ThenInclude(y => y.Privilege)
                    .IncludeAll(y => y.UserGroupMap)
                    .ThenInclude(y => y.Group)
                    .IncludeAll(y => y.UserRoleMap)
                    .ThenInclude(y => y.Role);
            this.detailsIncludes = indexIncludes;
            this.editIncludes = indexIncludes;
        }

        public async Task<IActionResult> Index()
        {
            var routine = new MvcRoutine(this, null);
            return await routine.HandleStorageAsync<IActionResult, User>(
                (repository) =>
                {
                    var entities = repository.List(indexIncludes);
                    return View(entities);
                });
        }

        public async Task<IActionResult> Details(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, User>(repository =>
            {
                var mvcTube = new MvcHandler(this);
                return mvcTube.Handle(
                    () => id != null,
                    () => repository.Find(e => e.UserId == id, detailsIncludes)
                );
            });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, User>(repository =>
            {
                var privilegesNavigation = new MvcNavigationFacade<User, Privilege, UserPrivilege, string>(
                    this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                    repository.Sprout<Privilege>().List()
                );

                var rolesNavigation = new MvcNavigationFacade<User, Role, UserRole, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Sprout<Role>().List()
                );

                var groupsNavigation = new MvcNavigationFacade<User, Group, UserGroup, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Sprout<Group>().List()
                );

                var mvcTube = new MvcHandler(this);
                return mvcTube.Handle(
                        () => id != null,
                        () => repository.Find(e => e.UserId == id, editIncludes),
                        (entity) =>
                        {
                            privilegesNavigation.Reset(entity.UserPrivilegeMap.Select(e => e.PrivilegeId));
                            rolesNavigation.Reset(entity.UserRoleMap.Select(e => e.RoleId));
                            groupsNavigation.Reset(entity.UserGroupMap.Select(e => e.GroupId));
                        }
                    );
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(BindedFields)] User entity)
        {
            var routine = new MvcRoutine(this, new { entity = entity });
            return await routine.HandleStorageAsync<IActionResult, User>(
                (repository, storage, state) =>
                {
                    if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
                        return Unauthorized();

                    var privilegesNavigation = new MvcNavigationFacade<User, Privilege, UserPrivilege, string>(
                        this, "Privileges", e => e.PrivilegeId, nameof(Privilege.PrivilegeName),
                        repository.Sprout<Privilege>().List()
                    );

                    privilegesNavigation.Parse(
                        e => new UserPrivilege() { UserId = entity.UserId, PrivilegeId = e.PrivilegeId },
                        s => s);

                    var rolesNavigation = new MvcNavigationFacade<User, Role, UserRole, int>(
                        this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                        repository.Sprout<Role>().List()
                    );

                    rolesNavigation.Parse(
                         e => new UserRole() { UserId = entity.UserId, RoleId = e.RoleId },
                         s => int.Parse(s)
                    );

                    var groupsNavigation = new MvcNavigationFacade<User, Group, UserGroup, int>(
                        this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                        repository.Sprout<Group>().List()
                    );

                    var mvcFork = new MvcHandler(this, ModelState.IsValid);
                    return mvcFork.Handle(
                        () => storage.Handle(
                            batch =>
                            {
                                batch.Modify(entity);
                                batch.UpdateRelations(entity,
                                    e => e.UserRoleMap,
                                    rolesNavigation.Selected,
                                    (e1, e2) => e1.RoleId == e2.RoleId
                                );
                                batch.UpdateRelations(entity,
                                    e => e.UserPrivilegeMap,
                                    privilegesNavigation.Selected,
                                    (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                                );
                            }),
                        () =>
                        {
                            privilegesNavigation.Reset();
                            rolesNavigation.Reset();
                            groupsNavigation.Reset();
                            return View(entity);
                        }
                    );
                });
        }
    }
}
