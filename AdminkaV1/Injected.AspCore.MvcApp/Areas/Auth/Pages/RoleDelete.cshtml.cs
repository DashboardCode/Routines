using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.Configuration.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class RoleDeleteModel : PageModel
    {
        public int Id { get; private set; }
        public Role Entity { get; private set; }

        readonly Func<Task<IActionResult>> delete;
        readonly Func<Task<IActionResult>> deleteConfirmed;

        public RoleDeleteModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.RoleMeta;
            delete = CrudRoutinePageConsumer<Role, int>.ComposeDelete(
                this,
                applicationSettings,
                routineResolvablesOption.Value,
                (e) => this.Entity = e,
                meta.DeleteIncludes, meta.KeyConverter, 
                meta.FindPredicate);

            deleteConfirmed = CrudRoutinePageConsumer<Role, int>.ComposeDeleteConfirmed(
                this,
                applicationSettings,
                routineResolvablesOption.Value,
                (e) => this.Entity = e,
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