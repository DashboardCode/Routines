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
            id => e => e.RoleId == id,
            Converters.TryParseInt,
            manyToMany => manyToMany
                .Add("Privileges",
                    repository => repository.Clone<Privilege>().List(),
                    nameof(Privilege.PrivilegeName),
                    e => e.RolePrivilegeMap,
                    e => e.PrivilegeId,
                    mm => mm.RoleId,
                    e => e.PrivilegeId,
                    (ep, ef) => new RolePrivilege() { RoleId = ep.RoleId, PrivilegeId = ef.PrivilegeId }
                 ).Add(
                    "Groups",
                    repository => repository.Clone<Group>().List(),
                    nameof(Group.GroupName),
                    e => e.GroupRoleMap,
                    mm => mm.GroupId,
                    mm => mm.RoleId,
                    e => e.GroupId,
                    (ep, ef) => new GroupRole() { RoleId = ep.RoleId, GroupId = ef.GroupId }
                 ).Add(
                    "Users",
                    repository => repository.Clone<User>().List(),
                    nameof(AuthenticationDom.User.LoginName),
                    e => e.UserRoleMap,
                    mm => mm.UserId,
                    mm => mm.RoleId,
                    e => e.UserId,
                    (ep, ef) => new UserRole() { RoleId = ep.RoleId, UserId = ef.UserId }
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