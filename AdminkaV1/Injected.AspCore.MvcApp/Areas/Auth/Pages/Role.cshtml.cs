using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Role Entity { get; private set; }

        readonly Func<Task<IActionResult>> details;
        public RoleModel()
        {
            var meta = Meta.RoleMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            details = CrudRoutinePageConsumer<Role, int>.ComposeDetails(
                this, 
                e => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return details();
        }
    }
}