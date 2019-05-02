using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    [ValidateAntiForgeryToken]
    public class RoleCreateModel : PageModel
    {
        public Role Entity { get; private set; }
        public string BackwardUrl { get; private set; }

        readonly static RoleMeta meta = Meta.RoleMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, defaultUrl: "Roles", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeCreate(
                (e) => Entity = e,
                authorize: null,
                meta.ReferencesCollection.PrepareEmptyOptions);
        }

        public Task<IActionResult> OnPostAsync()
        {
            var crud = new AdminkaCrudRoutinePageConsumer<Role, int>(this, defaultUrl: "Roles", backwardUrl => BackwardUrl = backwardUrl);
            return crud.ComposeCreateConfirmed(
                (e) => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelatedOnInsert);
        }
    }
}