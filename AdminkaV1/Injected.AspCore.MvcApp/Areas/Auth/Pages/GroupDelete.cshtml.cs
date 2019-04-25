using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupDeleteModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Group Entity { get; private set; }

        readonly Func<Task<IActionResult>> delete;
        readonly Func<Task<IActionResult>> deleteConfirmed;

        public GroupDeleteModel()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.GroupMeta;
            delete = CrudRoutinePageConsumer<Group, int>.ComposeDelete(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                meta.DeleteIncludes, meta.KeyConverter, 
                meta.FindPredicate);

            deleteConfirmed = CrudRoutinePageConsumer<Group, int>.ComposeDeleteConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
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