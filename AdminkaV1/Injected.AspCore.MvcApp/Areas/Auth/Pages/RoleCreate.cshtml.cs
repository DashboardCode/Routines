using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class RoleCreateModel : PageModel
    {
        readonly static RoleMeta meta = Meta.RoleMeta;

        public Role Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Role, int> Crud;

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, null, defaultUrl: "Roles", true);
            return Crud.HandleCreateAsync(
                (e) => Entity = e,
                authorize: null,
                meta.ReferencesCollection.PrepareEmptyOptions);
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, null, defaultUrl: "Roles", true);
            return Crud.HandleCreateConfirmedAsync(
                (e) => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity),
                meta.Constructor,
                meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert);
        }
    }
}