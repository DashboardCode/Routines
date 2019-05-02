using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class GroupCreateModel : PageModel
    {
        public Group Entity { get; private set; }
        public string BackwardUrl { get; set; }

        readonly static GroupMeta meta = Meta.GroupMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultUrl: "Groups", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeCreate(
                e => Entity = e,
                authorize: null,
                meta.ReferencesCollection.PrepareEmptyOptions
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultUrl: "Groups", (backwardUrl) => BackwardUrl = backwardUrl);
            return crud.ComposeCreateConfirmed(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert
            );
        }
    }
}