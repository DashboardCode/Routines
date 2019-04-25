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
    public class UsersModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public IEnumerable<User> List { get; private set; }

        Func<Task<IActionResult>> index;

        public UsersModel()
        {
            var meta = Meta.UserMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index = CrudRoutinePageConsumer<User, int>.ComposeIndex(this,
                l=> List = l,
                prf => BackwardUrl = prf.BackwardUrl,
                defaultBackwardUrl: null,
                meta.IndexIncludes);
        }

        public Task<IActionResult> OnGet()
        {
            return index();
        }
    }
}