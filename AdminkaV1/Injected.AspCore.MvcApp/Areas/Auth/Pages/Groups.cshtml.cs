using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupsModel : PageModel
    {
        public string BackwardUrl  { get; private set; }
        public IEnumerable<Group> List { get; private set; }

        Func<Task<IActionResult>> index;

        public GroupsModel()
        {
            var meta = Meta.GroupMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index = CrudRoutinePageConsumer<Group, int>.ComposeIndex(this,
                l => List = l,
                prf=> BackwardUrl= prf.BackwardUrl,
                defaultBackwardUrl: null,
                meta.IndexIncludes);
        }

        public Task<IActionResult> OnGet()
        {
            return index();
        }
    }
}