using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class GroupDeleteModel : PageModel, IGroupPartialModel
    {
        readonly static GroupMeta meta = Meta.GroupMeta;

        public Group Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<Group, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Group, int>(this, defaultReferrer: "Groups");
            return Crud.HandleDeleteAsync(
                e => Entity = e,
                authorize: null,
                meta.DeleteIncludes, 
                meta.KeyConverter,
                meta.FindPredicate
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Group, int>(this, defaultReferrer: "Groups");
            return Crud.HandleDeleteConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity), 
                meta.Constructor,
                meta.HiddenFormFields
            );
        }
    }
}