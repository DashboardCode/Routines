using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class PrivilegeModel : PageModel
    {
        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public Privilege Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Privilege, string> Crud;

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Privilege, string>(this, () => $"{nameof(Privilege)}?id={Entity.PrivilegeId}", defaultUrl: "Privileges", true);
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