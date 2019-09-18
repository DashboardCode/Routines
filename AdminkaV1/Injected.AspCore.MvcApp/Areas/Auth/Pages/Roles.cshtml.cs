using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RolesModel : PageModel
    {
        static readonly RoleMeta meta = Meta.RoleMeta;

        public IEnumerable<Role> List { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Role, int> Crud;

        public Task<IActionResult> OnGet()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, defaultReferrer: "/");
            return Crud.HandleIndexAsync(
                l => List = l,
                authorize: null,
                meta.IndexIncludes);
        }
    }
}