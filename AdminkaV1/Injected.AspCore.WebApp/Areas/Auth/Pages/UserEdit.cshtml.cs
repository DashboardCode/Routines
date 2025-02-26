using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class UserEditModel : PageModel
    {
        readonly static UserMeta meta = Meta.UserMeta;

        public User Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<User, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<User, int>(this, defaultReferrer: "Users");
            return Crud.HandleEditAsync(
                e => Entity = e,
                authorize: null,
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, 
                meta.ReferencesCollection.PrepareOptions);
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<User, int>(this, defaultReferrer: "Users");
            return Crud.HandleEditConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity), meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, 
                meta.ReferencesCollection.ParseRelatedOnUpdate);
        }
    }
}