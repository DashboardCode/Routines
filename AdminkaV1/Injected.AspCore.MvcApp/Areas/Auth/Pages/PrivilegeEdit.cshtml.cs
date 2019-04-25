using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class PrivilegeEditModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Privilege Entity { get; private set; }

        readonly Func<Task<IActionResult>> edit;
        readonly Func<Task<IActionResult>> editConfirmed;

        public PrivilegeEditModel()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.PrivilegeMeta;
            edit = CrudRoutinePageConsumer<Privilege, string>.ComposeEdit(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Privileges",
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
            editConfirmed = CrudRoutinePageConsumer<Privilege, string>.ComposeEditConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Privileges",
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