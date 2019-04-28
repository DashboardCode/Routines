using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Role Entity { get; private set; }

        readonly static RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var details = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeDetails(
                this,
                e => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize: null,
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return details();
        }
    }
}