using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupDeleteModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Group Entity { get; private set; }

        readonly static GroupMeta meta = Meta.GroupMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var delete = CrudRoutinePageConsumer<UserContext, User, Group, int>.ComposeDelete(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                authorize: null,
                meta.DeleteIncludes, meta.KeyConverter,
                meta.FindPredicate,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return delete();
        }

        public Task<IActionResult> OnPostAsync()
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var deleteConfirmed = CrudRoutinePageConsumer<UserContext, User, Group, int>.ComposeDeleteConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                authorize, meta.Constructor, meta.HiddenFormFields,
                MvcAppManager.CreateMetaPageRoutineHandler);
            return deleteConfirmed();
        }
    }
}