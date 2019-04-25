using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupCreateModel : PageModel
    {
        public Group Entity { get; private set; }
        public string BackwardUrl { get; set; }

        readonly Func<string, UserContext, bool> authorize;
        readonly GroupMeta meta;

        public GroupCreateModel()
        {
            authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            meta = Meta.GroupMeta;
        }

        public Task<IActionResult> OnGetAsync()
        {
            var insert = CrudRoutinePageConsumer<Group, int>.ComposeCreate(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                meta.ReferencesCollection.PrepareEmptyOptions);
            return insert();
        }

        public Task<IActionResult> OnPostAsync()
        {
            var insertConfirmed = CrudRoutinePageConsumer<Group, int>.ComposeCreateConfirmed(
                this,
                (e) => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Groups",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert);
            return insertConfirmed();
        }
    }
}