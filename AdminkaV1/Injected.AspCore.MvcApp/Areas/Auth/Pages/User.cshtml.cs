using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    public class UserModel : PageModel
    {
        readonly static UserMeta meta = Meta.UserMeta;

        public User Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<User, int> Crud;

        public Task<IActionResult> OnGetAsync()
        {
            var referrer = new AdminkaReferrer(this.HttpContext.Request, "Users", () => Entity.UserId.ToString(), "User");
            Crud = new AdminkaCrudRoutinePageConsumer<User, int>(this, referrer);
            return Crud.HandleDetailsAsync(
                e => Entity = e,
                authorize: null,
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
        }
    }
}