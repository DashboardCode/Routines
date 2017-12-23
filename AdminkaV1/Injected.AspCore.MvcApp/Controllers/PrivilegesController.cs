using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class PrivilegesController : RoutineController
    {
        #region Meta
        static ControllerMeta<Privilege, string> meta = new ControllerMeta<Privilege, string>(
            () => new Privilege(),
            id => e => e.PrivilegeId == id,
            Converters.TryParseString, 
            manyToMany=> manyToMany
                .Add("Roles",
                     repository => repository.Clone<Role>().List(),
                     e => e.RolePrivilegeMap,
                     e => e.RoleId,
                     new MvcNavigationFacade<Privilege, Role, RolePrivilege, int>(
                         e => e.RoleId,
                         nameof(Role.RoleName),
                         (ep, ef) => new RolePrivilege() { PrivilegeId = ep.PrivilegeId, RoleId = ef.RoleId },
                         s => int.Parse(s)
                     )
                     
                )
                .Add("Groups",
                     repository => repository.Clone<Group>().List(),
                     e => e.GroupPrivilegeMap,
                     e => e.GroupId,
                     new MvcNavigationFacade<Privilege, Group, GroupPrivilege, int>(
                          e => e.GroupId,
                         nameof(Group.GroupName),
                         (ep, ef) => new GroupPrivilege() { PrivilegeId = ep.PrivilegeId, GroupId = ef.GroupId },
                         s => int.Parse(s)
                     )
                     
                )
                .Add(
                    "Users",
                    repository => repository.Clone<User>().List(),
                    e => e.UserPrivilegeMap,
                    e => e.UserId,
                    new MvcNavigationFacade<Privilege, User, UserPrivilege, int>(
                         e => e.UserId,
                        nameof(AuthenticationDom.User.LoginName),
                        (ep, ef) => new UserPrivilege() { PrivilegeId = ep.PrivilegeId, UserId = ef.UserId },
                        s => int.Parse(s)
                    )
                    
                ),
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
            chain => chain.Include(e => e.PrivilegeName),
            editables => editables.Add(e=>e.PrivilegeName,  Binder.ConvertToString),
            notEditables => notEditables.Add(e => e.PrivilegeId).Add(e => e.RowVersion)
        );
        #endregion

        CrudRoutineControllerConsumer<Privilege, string> consumer;
        public PrivilegesController(IConfigurationRoot configurationRoot) :base(configurationRoot)
        {
            consumer = new CrudRoutineControllerConsumer<Privilege, string>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
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
    }
}
