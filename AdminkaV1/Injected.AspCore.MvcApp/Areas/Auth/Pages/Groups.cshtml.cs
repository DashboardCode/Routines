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
    public class GroupsModel : PageModel
    {
        public int Id { get; private set; }
        public IEnumerable<Group> List { get; private set; }

        Func<Task<IActionResult>> index;

        public GroupsModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            var meta = Meta.GroupMeta;
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index = CrudRoutinePageConsumer<Group, int>.ComposeIndex(this,
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