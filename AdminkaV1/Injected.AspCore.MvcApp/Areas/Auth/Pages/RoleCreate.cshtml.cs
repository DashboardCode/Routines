using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleCreateModel : PageModel
    {
        public Role Entity { get; private set; }
        public string BackwardUrl { get; private set; }

        readonly static RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var insert = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeCreate(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize: null,
                meta.ReferencesCollection.PrepareEmptyOptions,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return insert();
        }

        public Task<IActionResult> OnPostAsync()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var insertConfirmed = CrudRoutinePageConsumer<UserContext, User, Role, int>.ComposeCreateConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return insertConfirmed();
        }
    }
}