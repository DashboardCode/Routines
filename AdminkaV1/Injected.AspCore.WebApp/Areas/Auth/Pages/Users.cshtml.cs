using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Areas.Auth.Pages
{
    public class UsersModel : PageModel
    {
        readonly static UserMeta meta = Meta.UserMeta;
        public IEnumerable<User> List { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<User, int> Crud { get; private set; }

        public Task<IActionResult> OnGet()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<User, int>(this, defaultReferrer:"/");
            return Crud.HandleIndexAsync(
                l => List = l,
                authorize: null,
                meta.IndexIncludes
                );
        }
    }
}