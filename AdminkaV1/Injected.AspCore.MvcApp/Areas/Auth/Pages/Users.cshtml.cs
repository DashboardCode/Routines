using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class UsersModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public IEnumerable<User> List { get; private set; }

        readonly static UserMeta meta = Meta.UserMeta;

        public Task<IActionResult> OnGet()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<User, int>(this, defaultUrl: null, backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeIndex(
                l => List = l,
                authorize: null,
                meta.IndexIncludes
                );
        }
    }
}