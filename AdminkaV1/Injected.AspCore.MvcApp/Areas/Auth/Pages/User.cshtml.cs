using System;
using System.Threading.Tasks;
using DashboardCode.AdminkaV1.AuthenticationDom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class UserModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public User Entity { get; private set; }

        readonly Func<Task<IActionResult>> details;
        public UserModel()
        {
            var meta = Meta.UserMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            details = CrudRoutinePageConsumer<User, int>.ComposeDetails(
                this, 
                e => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Users",
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return details();
        }
    }
}