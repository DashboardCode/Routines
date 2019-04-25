using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleDeleteModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Role Entity { get; private set; }

        readonly Func<Task<IActionResult>> delete;
        readonly Func<Task<IActionResult>> deleteConfirmed;

        public RoleDeleteModel()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.RoleMeta;
            delete = CrudRoutinePageConsumer<Role, int>.ComposeDelete(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                meta.DeleteIncludes, meta.KeyConverter, 
                meta.FindPredicate);

            deleteConfirmed = CrudRoutinePageConsumer<Role, int>.ComposeDeleteConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Roles",
                authorize, meta.Constructor,  meta.HiddenFormFields);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return delete();
        }

        public Task<IActionResult> OnPostAsync()
        {
            return deleteConfirmed();
        }
    }
}