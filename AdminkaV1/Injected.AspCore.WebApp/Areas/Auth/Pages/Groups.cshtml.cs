using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    public class GroupsModel : PageModel
    {
        static readonly GroupMeta meta = Meta.GroupMeta;

        public IEnumerable<Group> List { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<Group, int> Crud { get; private set; }

        public Task<IActionResult> OnGet()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Group, int>(this, defaultReferrer: "/");
            return Crud.HandleIndexAsync(
                l => List = l,
                authorize: null,
                meta.IndexIncludes
            );
        }
    }
}