using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    public interface IRolePartialModel
    {
         Role Entity { get; }
        AdminkaCrudRoutinePageConsumer<Role, int> Crud { get; }
    }

    public class RoleModel : PageModel, IRolePartialModel
    {
        readonly static RoleMeta meta = Meta.RoleMeta;

        public Role Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Role, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            var referrer = new AdminkaReferrer(this.HttpContext.Request, "Roles", () => Entity.RoleId.ToString(), "Role");
            Crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, referrer);
            return Crud.HandleDetailsAsync(
                e => Entity = e,
                authorize: null,
                meta.DetailsIncludes, 
                meta.KeyConverter, 
                meta.FindPredicate
            );
        }
    }
}