using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupsModel : PageModel
    {
        public string BackwardUrl  { get; private set; }
        public IEnumerable<Group> List { get; private set; }

        static readonly GroupMeta meta = Meta.GroupMeta;

        public Task<IActionResult> OnGet()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultUrl: null, backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeIndex(
                l => List = l,
                authorize: null,
                meta.IndexIncludes
            );
        }
    }
}