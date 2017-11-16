﻿using System.Linq;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler
using Microsoft.Extensions.Configuration;
using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class PrivilegesController : RoutineController
    {
        const string BindedFields = nameof(Privilege.PrivilegeId) + ", " + nameof(Privilege.PrivilegeName);
        Include<Privilege> indexIncludes;
        Include<Privilege> detailsIncludes;
        Include<Privilege> editIncludes;
        public PrivilegesController(IConfigurationRoot configurationRoot):base(configurationRoot)
        {
            this.indexIncludes = includable =>
                includable.IncludeAll(y => y.RolePrivilegeMap)
                    .ThenInclude(y => y.Role)
                    .IncludeAll(y => y.UserPrivilegeMap)
                    .ThenInclude(y => y.User)
                    .IncludeAll(y => y.GroupPrivilegeMap)
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
                    var entities = repository.List(indexIncludes);
                    return View(entities);
                });
        }

        public async Task<IActionResult> Details(string id)
        {
            var routine = new MvcRoutine(this, new { id = id });
            return await routine.HandleStorageAsync<IActionResult, Privilege>(repository =>
            {
                var mvcTube = new MvcHandler(this);
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
                var rolesNavigation = new MvcNavigationFacade<Privilege, Role, RolePrivilege, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Clone<Role>().List()
                    );
                var groupsNavigation = new MvcNavigationFacade<Privilege, Group, GroupRole, int>(
                    this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                    repository.Clone<Group>().List()
                );
                var usersNavigation = new MvcNavigationFacade<Privilege, User, UserRole, int>(
                   this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
                   repository.Clone<User>().List()
                );

                var mvcTube = new MvcHandler(this);
                return mvcTube.Handle(
                        () => id != null,
                        () => repository.Find(e => e.PrivilegeId == id, editIncludes),
                        (entity) =>
                        {
                            rolesNavigation.Reset(entity.RolePrivilegeMap.Select(e => e.RoleId));
                            groupsNavigation.Reset(entity.GroupPrivilegeMap.Select(e => e.GroupId));
                            usersNavigation.Reset(entity.UserPrivilegeMap.Select(e => e.UserId));
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

                var rolesNavigation = new MvcNavigationFacade<Privilege, Role, RolePrivilege, int>(
                    this, "Roles", e => e.RoleId, nameof(Role.RoleName),
                    repository.Clone<Role>().List()
                );
                rolesNavigation.Parse(
                    e => new RolePrivilege() { PrivilegeId = entity.PrivilegeId, RoleId = e.RoleId },
                    s => int.Parse(s));

                var groupsNavigation = new MvcNavigationFacade<Privilege, Group, GroupPrivilege, int>(
                     this, "Groups", e => e.GroupId, nameof(Group.GroupName),
                     repository.Clone<Group>().List()
                 );
                groupsNavigation.Parse(
                                    e => new GroupPrivilege() { PrivilegeId = entity.PrivilegeId, GroupId = e.GroupId },
                                    s => int.Parse(s));

                var usersNavigation = new MvcNavigationFacade<Privilege, User, UserPrivilege, int>(
                    this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
                    repository.Clone<User>().List()
                 );

                usersNavigation.Parse(
                                    e => new UserPrivilege() { PrivilegeId = entity.PrivilegeId, UserId = e.UserId },
                                    s => int.Parse(s));

                var mvcFork = new MvcHandler(this, ModelState.IsValid);
                return mvcFork.Handle(
                    () => storage.Handle(
                        batch =>
                        {
                            batch.Modify(entity);
                            batch.ModifyWithRelated(entity,
                                e => e.GroupPrivilegeMap,
                                groupsNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                            batch.ModifyWithRelated(entity,
                                e => e.RolePrivilegeMap,
                                rolesNavigation.Selected,
                                (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
                            );
                            batch.ModifyWithRelated(entity,
                                e => e.UserPrivilegeMap,
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
