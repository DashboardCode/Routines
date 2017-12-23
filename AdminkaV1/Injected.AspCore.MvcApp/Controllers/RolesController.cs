using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.AdminkaV1.AuthenticationDom; 
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class RolesController : RoutineController
    {
        #region Meta
        static IAdminkaBrowserMetaService metaBrowser;
        static ControllerMeta<Role, int> meta = new ControllerMeta<Role, int>(
            () => new Role(),
            id => e => e.RoleId == id,
            Converters.TryParseInt,
             manyToMany => manyToMany
                .Add("Privileges",
                    repository => repository.Clone<Privilege>().List(),
                    e => e.RolePrivilegeMap,
                    //(e1, e2) => e1.PrivilegeId == e2.PrivilegeId,
                    e => e.PrivilegeId,
                    new MvcNavigationFacade<Role, Privilege, RolePrivilege, string>(
                         e => e.PrivilegeId,
                        nameof(Privilege.PrivilegeName),
                        (ep, ef) => new RolePrivilege() { RoleId = ep.RoleId, PrivilegeId = ef.PrivilegeId },
                        s => s
                    )
                    
                 ).Add(
                     "Groups",
                     repository => repository.Clone<Group>().List(),
                    e => e.GroupRoleMap,
                    //(e1, e2) => e1.RoleId == e2.RoleId,
                    e => e.GroupId,
                     new MvcNavigationFacade<Role, Group, GroupRole, int>(
                        e => e.GroupId,
                        nameof(Group.GroupName),
                        (ep, ef) => new GroupRole() { RoleId = ep.RoleId, GroupId = ef.GroupId },
                        s => int.Parse(s)
                    )
                    
                 ).Add(
                    "Users",
                    repository => repository.Clone<User>().List(),
                    e => e.UserRoleMap,
                    //(e1, e2) => e1.UserId == e2.UserId,
                    e => e.UserId,
                    new MvcNavigationFacade<Role, User, UserRole, int>(
                        e => e.UserId,
                        nameof(AuthenticationDom.User.LoginName),
                        (ep, ef) => new UserRole() { RoleId = ep.RoleId, UserId = ef.UserId },
                        s => int.Parse(s)
                    )
            ),
            chain => chain
                       .IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.GroupRoleMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.User),
            chain => chain.IncludeAll(e => e.RolePrivilegeMap)
                       .ThenInclude(e => e.Privilege)
                       .IncludeAll(e => e.GroupRoleMap)
                       .ThenInclude(e => e.Group)
                       .IncludeAll(e => e.UserRoleMap)
                       .ThenInclude(e => e.User),
            null,
            editables =>
                editables.Add(e=>e.RoleName, Binder.ConvertToString),
            
            notEditables => notEditables.Add(e => e.RoleId).Add(e => e.RowVersion)
        );
        #endregion

        CrudRoutineControllerConsumer<Role, int> consumer;
        public RolesController(IConfigurationRoot configurationRoot) : base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<Role, int>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public async Task<IActionResult> Details()
        {
            return await consumer.Details();
        }

        public async Task<IActionResult> Index()
        {
            return await consumer.Index();
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            return await consumer.Create();
        }

        [HttpPost, ActionName(nameof(Create)), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed()
        {
            return await consumer.CreateFormData();
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
            return await consumer.EditFormData();
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete()
        {
            return await consumer.Delete();
        }

        [HttpPost, ActionName(nameof(Delete)), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFormData()
        {
            return await consumer.DeleteFormData();
        }
        #endregion
     }
}