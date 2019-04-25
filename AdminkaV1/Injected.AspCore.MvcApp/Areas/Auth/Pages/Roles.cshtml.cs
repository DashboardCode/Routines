using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.Configuration.Standard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RolesModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public IEnumerable<Role> List { get; private set; }

        Func<Task<IActionResult>> index;

        public RolesModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            var meta = Meta.RoleMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index = CrudRoutinePageConsumer<Role, int>.ComposeIndex(this,
                l=> List = l,
                prf => BackwardUrl = prf.BackwardUrl,
                null,
                meta.IndexIncludes);
        }

        public Task<IActionResult> OnGet()
        {
            return index();
        }
    }
}