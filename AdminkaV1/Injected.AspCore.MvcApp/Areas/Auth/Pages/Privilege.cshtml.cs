using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class PrivilegeModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Privilege Entity { get; private set; }

        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Privilege, string>(this, defaultUrl: "Privileges", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeDetails(
                e => Entity = e,
                userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.DetailsIncludes, 
                meta.KeyConverter, 
                meta.FindPredicate
            );
        }
    }
}