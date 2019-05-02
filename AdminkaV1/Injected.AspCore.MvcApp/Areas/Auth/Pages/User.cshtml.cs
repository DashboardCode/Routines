using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            var crud = new AdminkaCrudRoutinePageConsumer<User, int>(this, defaultUrl: "Users", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeDetails(
                e => Entity = e,
                authorize: null,
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
        }
    }
}