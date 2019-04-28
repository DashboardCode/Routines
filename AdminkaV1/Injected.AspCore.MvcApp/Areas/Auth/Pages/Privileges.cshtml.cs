using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class PrivilegesModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public IEnumerable<Privilege> List { get; private set; }

        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public Task<IActionResult> OnGet()
        {
            var index = CrudRoutinePageConsumer<UserContext, User, Privilege, string>.ComposeIndex(this,
                l => List = l,
                prf => BackwardUrl = prf.BackwardUrl,
                defaultBackwardUrl: null,
                authorize: null,
                meta.IndexIncludes,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return index();
        }
    }
}