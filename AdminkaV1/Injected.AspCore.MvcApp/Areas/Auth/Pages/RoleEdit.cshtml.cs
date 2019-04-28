using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleEditModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Role Entity { get; private set; }

        readonly static RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var edit = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeEdit(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                authorize: null,
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return edit();
        }

        public Task<IActionResult> OnPostAsync()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var editConfirmed = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeEditConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, meta.ReferencesCollection.ParseRelatedOnUpdate,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return editConfirmed();
        }
    }
}