using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class UserModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public User Entity { get; private set; }

        readonly static UserMeta meta = Meta.UserMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var details = CrudRoutinePageConsumer<UserContext, User, User, int>.ComposeDetails(
                this,
                e => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Users",
                authorize: null,
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return details();
        }
    }
}