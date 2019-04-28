using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleDeleteModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Role Entity { get; private set; }

        readonly static RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var delete = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeDelete(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize: null,
                meta.DeleteIncludes, meta.KeyConverter,
                meta.FindPredicate,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return delete();
        }

        public Task<IActionResult> OnPostAsync()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var deleteConfirmed = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeDeleteConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize, meta.Constructor, meta.HiddenFormFields,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return deleteConfirmed();
        }
    }
}