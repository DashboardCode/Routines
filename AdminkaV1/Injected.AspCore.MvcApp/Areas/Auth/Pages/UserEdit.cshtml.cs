using System;
using System.Threading.Tasks;
using DashboardCode.AdminkaV1.AuthenticationDom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class UserEditModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public User Entity { get; private set; }

        readonly Func<Task<IActionResult>> edit;
        readonly Func<Task<IActionResult>> editConfirmed;

        public UserEditModel()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.UserMeta;
            edit = CrudRoutinePageConsumer<User, int>.ComposeEdit(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Users",
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
            editConfirmed = CrudRoutinePageConsumer<User, int>.ComposeEditConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Users",
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