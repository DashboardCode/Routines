using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    public class PrivilegeModel : PageModel
    {
        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public Privilege Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<Privilege, string> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            var referrer = new AdminkaReferrer(this.HttpContext.Request, "Privileges", () => Entity.PrivilegeId.ToString(), "Privilege");
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Privilege, string>(this, referrer);
            return Crud.HandleDetailsAsync(
                e => Entity = e,
                userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.DetailsIncludes, 
                meta.KeyConverter, 
                meta.FindPredicate
            );
        }
    }
}