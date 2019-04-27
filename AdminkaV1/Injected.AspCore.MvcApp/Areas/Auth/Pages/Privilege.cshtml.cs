using System;
using System.Threading.Tasks;
using DashboardCode.AdminkaV1.AuthenticationDom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class PrivilegeModel : PageModel
    {
        public string BackwardUrl { get; private set; }
        public Privilege Entity { get; private set; }

        readonly static PrivilegeMeta meta = Meta.PrivilegeMeta;

        public Task<IActionResult> OnGetAsync()
        {
            var details = CrudRoutinePageConsumer<Privilege, string>.ComposeDetails(
                this,
                e => this.Entity = e,
                prf => BackwardUrl = prf.BackwardUrl,
                "Privileges",
                authorize: null,
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
            return details();
        }
    }
}