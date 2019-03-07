using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.Standard;

using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class PrivilegesController : ConfigurableController
    {
        #region Meta
        static PrivilegeMeta meta = new PrivilegeMeta();
        #endregion

        CrudRoutineControllerConsumer<Privilege, string> consumer;
        public PrivilegesController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) :base(applicationSettings, routineResolvablesOption.Value)
        {
            consumer = new CrudRoutineControllerConsumer<Privilege, string>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public Task<IActionResult> Details()
        {
            return consumer.Details();
        }

        public Task<IActionResult> Index()
        {
            return consumer.Index();
        }
        #endregion

        #region Edit
        public Task<IActionResult> Edit()
        {
            return consumer.Edit();
        }

        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public Task<IActionResult> EditFormData()
        {
            return consumer.EditConfirmed();
        }
        #endregion
    }
}
