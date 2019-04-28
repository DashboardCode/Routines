using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RolesModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public IEnumerable<Role> List { get; private set; }

        static readonly RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGet()
        {
            var index = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeIndex(this,
                l => List = l,
                prf => BackwardUrl = prf.BackwardUrl,
                null,
                authorize: null,
                meta.IndexIncludes,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return index();
        }
    }
}