using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupModel : PageModel
    {
        public Group Entity { get; private set; }
        public string BackwardUrl { get; private set; }

        readonly static GroupMeta meta = Meta.GroupMeta;
        
        public Task<IActionResult> OnGetAsync()
        {
            var details = CrudRoutinePageConsumer<Group, int>.ComposeDetails(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.DetailsIncludes,
                meta.KeyConverter,
                meta.FindPredicate);
            return details();
        }
    }
}