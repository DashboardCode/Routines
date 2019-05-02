using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class PrivilegeEditModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Privilege Entity { get; private set; }

        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Privilege, string>(this, defaultUrl: "Privileges", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeEdit(
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
            var crud = new AdminkaCrudRoutinePageConsumer<Privilege, string>(this, defaultUrl: "Privileges", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeEditConfirmed(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem), 
                meta.Constructor, 
                meta.FormFields, 
                meta.HiddenFormFields, 
                meta.DisabledFormFields, 
                meta.ReferencesCollection.ParseRelatedOnUpdate
            );
        }
    }
}