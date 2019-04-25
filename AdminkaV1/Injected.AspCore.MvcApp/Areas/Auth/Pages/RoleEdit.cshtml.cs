using System;
using System.Threading.Tasks;
using DashboardCode.AdminkaV1.AuthenticationDom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleEditModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Role Entity { get; private set; }

        readonly Func<Task<IActionResult>> edit;
        readonly Func<Task<IActionResult>> editConfirmed;

        public RoleEditModel()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.RoleMeta;
            edit = CrudRoutinePageConsumer<Role, int>.ComposeEdit(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
            editConfirmed = CrudRoutinePageConsumer<Role, int>.ComposeEditConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, meta.ReferencesCollection.ParseRelatedOnUpdate);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return edit();
        }

        public Task<IActionResult> OnPostAsync()
        {
            return editConfirmed();
        }
    }
}