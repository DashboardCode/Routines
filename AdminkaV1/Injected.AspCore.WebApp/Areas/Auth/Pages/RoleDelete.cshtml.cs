using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class RoleDeleteModel : PageModel, IRolePartialModel
    {
        readonly static RoleMeta meta = Meta.RoleMeta;

        public Role Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<Role, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Role, int>(this, defaultReferrer: "Roles");
            return Crud.HandleDeleteAsync(
                (e) => Entity = e,
                authorize: null,
                meta.DeleteIncludes, meta.KeyConverter, meta.FindPredicate
                );
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Role, int>(this, defaultReferrer: "Roles");
            return Crud.HandleDeleteConfirmedAsync(
                (e) => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity), meta.Constructor, meta.HiddenFormFields);
        }
    }
}