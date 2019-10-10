using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class GroupCreateModel : PageModel, IGroupEditPartialModel
    {
        readonly static GroupMeta meta = Meta.GroupMeta;

        public Group Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Group, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultReferrer: "Groups");
            return Crud.HandleCreateAsync(
                e => Entity = e,
                authorize: null,
                meta.ReferencesCollection.PrepareEmptyOptions
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultReferrer:"Groups");
            return Crud.HandleCreateConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity), meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert
            );
        }
    }
}