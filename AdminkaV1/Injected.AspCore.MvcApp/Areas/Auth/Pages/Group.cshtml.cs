using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.Configuration.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupModel : PageModel
    {
        public int Id { get; private set; }
        public Group Entity { get; private set; }

        readonly Func<Task<IActionResult>> details;
        public GroupModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            var meta = Meta.GroupMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            details = CrudRoutinePageConsumer<Group, int>.ComposeDetails(
                this, applicationSettings, routineResolvablesOption.Value,
                e => this.Entity = e,
                meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return details();
        }
    }
}