using System;
using System.Threading.Tasks; // assync actions
using Microsoft.AspNetCore.Mvc; // controler

using DashboardCode.AdminkaV1.AuthenticationDom; // entity
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using DashboardCode.Routines.Configuration.Standard;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class UsersController : ConfigurableController
    {
        #region Meta
        static UserMeta meta = new UserMeta();
        #endregion

        Func<Task<IActionResult>> index;
        Func<Task<IActionResult>> details;
        Func<Task<IActionResult>> edit;
        Func<Task<IActionResult>> editConfirmed;
        public UsersController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) :base(applicationSettings, routineResolvablesOption.Value)
        {
            Func<string, UserContext, bool> authorize = (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem);
            index   = CrudRoutineControllerConsumer<User, int>.ComposeIndex(this, meta.IndexIncludes);
            details = CrudRoutineControllerConsumer<User, int>.ComposeDetails(
                this, meta.DetailsIncludes, meta.KeyConverter, meta.FindPredicate);
            edit = CrudRoutineControllerConsumer<User, int>.ComposeEdit(
                this, meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, meta.ReferencesCollection.PrepareOptions);
            editConfirmed = CrudRoutineControllerConsumer<User, int>.ComposeEditConfirmed(
                this, authorize, meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, meta.ReferencesCollection.ParseRelated);
        }

        #region Details / Index
        public Task<IActionResult> Details()
            => details();

        public Task<IActionResult> Index()
            => index();
        #endregion

        #region Edit
        public Task<IActionResult> Edit()
            => edit();
 
        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public Task<IActionResult> EditFormData()
            => editConfirmed();
        #endregion
    }
}