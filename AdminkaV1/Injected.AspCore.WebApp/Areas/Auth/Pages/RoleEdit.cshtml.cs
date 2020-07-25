using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Areas.Auth.Pages
{
    public interface IRoleEditPartialModel
    {
        Role Entity { get; }
    }

    [ValidateAntiForgeryToken]
    public class RoleEditModel : PageModel, IRoleEditPartialModel
    {
        readonly static RoleMeta meta = Meta.RoleMeta;
        public Role Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<Role, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Role, int>(this, defaultReferrer: "Roles");
            return Crud.HandleEditAsync(
                e => Entity = e,
                authorize: null,
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Role, int>(this, defaultReferrer: "Roles");
            return Crud.HandleEditConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity),
                meta.Constructor,
                meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, meta.ReferencesCollection.ParseRelatedOnUpdate);
        }
    }
}