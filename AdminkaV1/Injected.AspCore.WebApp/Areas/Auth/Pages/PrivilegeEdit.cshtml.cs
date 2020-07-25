using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class PrivilegeEditModel : PageModel
    {
        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;
        
        public Privilege Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumerAsync<Privilege, string> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Privilege, string>(this, defaultReferrer: "Privileges");
            return Crud.HandleEditAsync(
                e => Entity = e,
                authorize: null,
                meta.EditIncludes, 
                meta.KeyConverter, 
                meta.FindPredicate, 
                meta.ReferencesCollection.PrepareOptions
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumerAsync<Privilege, string>(this, defaultReferrer: "Privileges");
            return Crud.HandleEditConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity),
                meta.Constructor,
                meta.FormFields, 
                meta.HiddenFormFields, 
                meta.DisabledFormFields, 
                meta.ReferencesCollection.ParseRelatedOnUpdate
            );
        }
    }
}