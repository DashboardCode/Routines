using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class GroupDeleteModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Group Entity { get; private set; }

        readonly static GroupMeta meta = Meta.GroupMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultUrl: "Groups", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeDelete(
                e => Entity = e,
                authorize: null,
                meta.DeleteIncludes, 
                meta.KeyConverter,
                meta.FindPredicate
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultUrl: "Groups", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeDeleteConfirmed(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem), 
                meta.Constructor, 
                meta.HiddenFormFields
            );
        }
    }
}