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
    public class PrivilegesModel : PageModel
    {
        public int Id { get; private set; }
        public IEnumerable<Privilege> List { get; private set; }

        public readonly Func<Task<IActionResult>> index;

        public PrivilegesModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            var meta = Meta.PrivilegeMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index = CrudRoutinePageConsumer<Privilege, string>.ComposeIndex(this,
                applicationSettings,
                routineResolvablesOption.Value,
                l=> List = l,
                meta.IndexIncludes);
        }

        public Task<IActionResult> OnGet()
        {
            return index();
        }
    }
}