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
    public class GroupCreateModel : PageModel
    {
        public Group Entity { get; private set; }

        readonly Func<Task<IActionResult>> insert;
        readonly Func<Task<IActionResult>> insertConfirmed;

        public GroupCreateModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.GroupMeta;

            insert = CrudRoutinePageConsumer<Group, int>.ComposeCreate(
                this,
                applicationSettings,
                routineResolvablesOption.Value,
                (e) => this.Entity = e,
                meta.ReferencesCollection.PrepareEmptyOptions);

            insertConfirmed = CrudRoutinePageConsumer<Group, int>.ComposeCreateConfirmed(
                this,
                applicationSettings,
                routineResolvablesOption.Value,
                (e) => this.Entity = e,
                "Groups",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.ReferencesCollection.ParseRelated);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return insert();
        }

        public Task<IActionResult> OnPostAsync()
        {
            return insertConfirmed();
        }
    }
}