using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupsModel : PageModel
    {
        static readonly GroupMeta meta = Meta.GroupMeta;

        public IEnumerable<Group> List { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Group, int> Crud;

        public Task<IActionResult> OnGet()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, null, defaultUrl: null, true);
            return Crud.HandleIndexAsync(
                l => List = l,
                authorize: null,
                meta.IndexIncludes
            );
        }
    }
}