using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.Configuration.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Areas.Auth.Pages
{
    public class GroupEditModel : PageModel
    {
        public int Id { get; private set; }
        public Group Entity { get; private set; }

        readonly Func<Task<IActionResult>> edit;
        readonly Func<Task<IActionResult>> editConfirmed;

        public GroupEditModel(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption)
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            var meta = Meta.GroupMeta;
            edit = CrudRoutinePageConsumer<Group, int>.ComposeEdit(
                this,
                applicationSettings,
                routineResolvablesOption.Value,
                (e) => this.Entity = e,
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
            editConfirmed = CrudRoutinePageConsumer<Group, int>.ComposeEditConfirmed(
                this,
                applicationSettings,
                routineResolvablesOption.Value,
                (e) => this.Entity = e,
                "Groups",
                authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, meta.ReferencesCollection.ParseRelated);
        }

        public Task<IActionResult> OnGetAsync()
        {
            return edit();
        }

        public Task<IActionResult> OnPostAsync()
        {
            return editConfirmed();
        }
    }
}