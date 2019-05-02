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

        readonly static RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, defaultUrl: null, backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeDetails(
                e => Entity = e,
                authorize: null,
                meta.DetailsIncludes, 
                meta.KeyConverter, 
                meta.FindPredicate
            );
        }
    }
}