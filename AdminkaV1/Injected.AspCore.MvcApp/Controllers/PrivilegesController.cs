using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class PrivilegesController : RoutineController
    {
        #region Meta
        static ControllerMeta<Privilege, string> meta = new ControllerMeta<Privilege, string>(
            () => new Privilege(),
            new ReferencesMeta<Privilege>(new IManyToMany<Privilege>[] {
                new ManyToMany<Privilege, Role, RolePrivilege, int>(
                    new MvcNavigationFacade2<Privilege, Role, RolePrivilege, int>(
                        "Roles", e => e.RoleId,
                        nameof(Role.RoleName),
                        (ep, ef) => new RolePrivilege() { PrivilegeId = ep.PrivilegeId, RoleId = ef.RoleId },
                        s => int.Parse(s)
                    ),
                    repository=>repository.Clone<Role>().List(),
                    e => e.RolePrivilegeMap,
                    (e1, e2) => e1.RoleId == e2.RoleId,
                    e => e.RoleId),

                new ManyToMany<Privilege, Group, GroupPrivilege, int>(
                    new MvcNavigationFacade2<Privilege, Group, GroupPrivilege, int>(
                        "Groups", e => e.GroupId,
                        nameof(Group.GroupName),
                        (ep, ef) => new GroupPrivilege() { PrivilegeId = ep.PrivilegeId, GroupId = ef.GroupId },
                        s => int.Parse(s)
                    ), 
                    repository=>repository.Clone<Group>().List(),
                    e => e.GroupPrivilegeMap,
                    (e1, e2) => e1.PrivilegeId == e2.PrivilegeId,  
                    e => e.GroupId),

                new ManyToMany<Privilege, User, UserPrivilege, int>(
                    new MvcNavigationFacade2<Privilege, User, UserPrivilege, int>(
                        "Users", e => e.UserId,
                        nameof(AuthenticationDom.User.LoginName),
                        (ep, ef) => new UserPrivilege() { PrivilegeId = ep.PrivilegeId, UserId = ef.UserId },
                        s => int.Parse(s)
                    ),
                    repository=>repository.Clone<User>().List(),
                    e => e.UserPrivilegeMap,
                    (e1, e2) => e1.PrivilegeId == e2.PrivilegeId,
                    e => e.UserId),
            }),
            chain => chain.IncludeAll(e => e.GroupPrivilegeMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.User),
            chain => chain.IncludeAll(e => e.GroupPrivilegeMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Role)
                       .IncludeAll(e => e.UserPrivilegeMap)
                       .ThenInclude(e => e.User),
            id => e => e.PrivilegeId == id,
            chain => chain.Include(e => e.PrivilegeName),
            editables => editables.Add(e=>e.PrivilegeName,  setter=>e => sv => Binder.TryString(sv, v => setter(e,v))),
            notEditables => notEditables.Add(e => e.PrivilegeId).Add(e => e.RowVersion)
            
        );
        #endregion

        

        CrudRoutineControllerConsumer<Privilege, string> consumer;
        public PrivilegesController(IConfigurationRoot configurationRoot) :base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<Privilege, string>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public async Task<IActionResult> Details(string id)
        {
            return await consumer.Details(id);
        }

        public async Task<IActionResult> Index()
        {
            return await consumer.Index();
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(string id)
        {
            return await consumer.Edit(id);
        }

        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed()
        {
            return await consumer.EditConfirmed();
        }
        #endregion


        //const string BindedFields = nameof(Privilege.PrivilegeId) + ", " + nameof(Privilege.PrivilegeName);
        //Include<Privilege> indexIncludes;
        //Include<Privilege> detailsIncludes;
        //Include<Privilege> editIncludes;
        //public PrivilegesController(IConfigurationRoot configurationRoot):base(configurationRoot)
        //{
        //    this.indexIncludes = includable =>
        //        includable.IncludeAll(y => y.RolePrivilegeMap)
        //            .ThenInclude(y => y.Role)
        //            .IncludeAll(y => y.UserPrivilegeMap)
        //            .ThenInclude(y => y.User)
        //            .IncludeAll(y => y.GroupPrivilegeMap)
        //            .ThenInclude(y => y.Group);
        //    this.detailsIncludes = indexIncludes;
        //    this.editIncludes = indexIncludes;
        //}

        //public async Task<IActionResult> Index()
        //{
        //    var routine = new MvcRoutine(this, null);
        //    return await routine.HandleStorageAsync<IActionResult, Privilege>(
        //        (repository) =>
        //        {
        //            var entities = repository.List(indexIncludes);
        //            return View(entities);
        //        });
        //}

        //public async Task<IActionResult> Details(string id)
        //{
        //    var routine = new MvcRoutine(this, new { id = id });
        //    return await routine.HandleStorageAsync<IActionResult, Privilege>(repository =>
        //    {
        //        return this.MakeActionResultOnRequest(
        //            () => id != null,
        //            () => repository.Find(e => e.PrivilegeId == id, detailsIncludes)
        //        );
        //    });
        //}

        //public async Task<IActionResult> Edit(string id)
        //{
        //    var routine = new MvcRoutine(this, new { id = id });
        //    return await routine.HandleStorageAsync<IActionResult, Privilege>(repository =>
        //    {
        //        var rolesNavigation = new MvcNavigationFacade<Privilege, Role, RolePrivilege, int>(
        //            this, "Roles", e => e.RoleId, nameof(Role.RoleName),
        //            repository.Clone<Role>().List()
        //            );
        //        var groupsNavigation = new MvcNavigationFacade<Privilege, Group, GroupRole, int>(
        //            this, "Groups", e => e.GroupId, nameof(Group.GroupName),
        //            repository.Clone<Group>().List()
        //        );
        //        var usersNavigation = new MvcNavigationFacade<Privilege, User, UserRole, int>(
        //           this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
        //           repository.Clone<User>().List()
        //        );

        //        return this.MakeActionResultOnRequest(
        //                () => id != null,
        //                () => repository.Find(e => e.PrivilegeId == id, editIncludes),
        //                (entity) =>
        //                {
        //                    rolesNavigation.Reset(entity.RolePrivilegeMap.Select(e => e.RoleId));
        //                    groupsNavigation.Reset(entity.GroupPrivilegeMap.Select(e => e.GroupId));
        //                    usersNavigation.Reset(entity.UserPrivilegeMap.Select(e => e.UserId));
        //                }
        //            );
        //    });
        //}
        //[HttpPost, ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit([Bind(BindedFields)] Privilege entity)
        //{
        //    var routine = new MvcRoutine(this, new { entity = entity });
        //    return await routine.HandleStorageAsync<IActionResult, Privilege>(
        //        (repository, storage, state) =>
        //    {
        //        if (!state.UserContext.HasPrivilege(Privilege.ConfigureSystem))
        //            return Unauthorized();

        //        var rolesNavigation = new MvcNavigationFacade<Privilege, Role, RolePrivilege, int>(
        //            this, "Roles", e => e.RoleId, nameof(Role.RoleName),
        //            repository.Clone<Role>().List()
        //        );
        //        rolesNavigation.Parse(
        //            e => new RolePrivilege() { PrivilegeId = entity.PrivilegeId, RoleId = e.RoleId },
        //            s => int.Parse(s));

        //        var groupsNavigation = new MvcNavigationFacade<Privilege, Group, GroupPrivilege, int>(
        //             this, "Groups", e => e.GroupId, nameof(Group.GroupName),
        //             repository.Clone<Group>().List()
        //         );
        //        groupsNavigation.Parse(
        //                            e => new GroupPrivilege() { PrivilegeId = entity.PrivilegeId, GroupId = e.GroupId },
        //                            s => int.Parse(s));

        //        var usersNavigation = new MvcNavigationFacade<Privilege, User, UserPrivilege, int>(
        //            this, "Users", e => e.UserId, nameof(AuthenticationDom.User.LoginName),
        //            repository.Clone<User>().List()
        //         );

        //        usersNavigation.Parse(
        //                            e => new UserPrivilege() { PrivilegeId = entity.PrivilegeId, UserId = e.UserId },
        //                            s => int.Parse(s));

        //        return this.MakeActionResultOnSave(
        //            ModelState.IsValid, 
        //            () => storage.Handle(
        //                batch =>
        //                {
        //                    batch.Modify(entity);
        //                    batch.ModifyRelated(entity,
        //                        e => e.GroupPrivilegeMap,
        //                        groupsNavigation.Selected,
        //                        (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
        //                    );
        //                    batch.ModifyRelated(entity,
        //                        e => e.RolePrivilegeMap,
        //                        rolesNavigation.Selected,
        //                        (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
        //                    );
        //                    batch.ModifyRelated(entity,
        //                        e => e.UserPrivilegeMap,
        //                        usersNavigation.Selected,
        //                        (e1, e2) => e1.PrivilegeId == e2.PrivilegeId
        //                    );
        //                }),
        //            () =>
        //            {
        //                rolesNavigation.Reset();
        //                groupsNavigation.Reset();
        //                usersNavigation.Reset();
        //                return View(entity);
        //            }
        //        );
        //    });
        //}
    }
}
