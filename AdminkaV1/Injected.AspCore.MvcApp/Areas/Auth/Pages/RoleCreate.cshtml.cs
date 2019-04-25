using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleCreateModel : PageModel
    {
        public Role Entity { get; private set; }
        public string BackwardUrl { get; private set; }
        readonly Func<Task<IActionResult>> insert;
        readonly Func<Task<IActionResult>> insertConfirmed;

        public RoleCreateModel()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.RoleMeta;

            insert = CrudRoutinePageConsumer<Role, int>.ComposeCreate(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                meta.ReferencesCollection.PrepareEmptyOptions);

            insertConfirmed = CrudRoutinePageConsumer<Role, int>.ComposeCreateConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return insert();
        }

        public Task<IActionResult> OnPostAsync()
        {
            return insertConfirmed();
        }
    }
}