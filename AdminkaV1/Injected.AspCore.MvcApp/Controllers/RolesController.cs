using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration.Standard;

using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public class RolesController : ConfigurableController
    {
        #region Meta
        static RoleMeta meta = new RoleMeta();
        #endregion

        CrudRoutineControllerConsumer<Role, int> consumer;
        public RolesController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) : base(applicationSettings, routineResolvablesOption.Value)
        {
            consumer = new CrudRoutineControllerConsumer<Role, int>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
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

        #region Create
        public Task<IActionResult> Create()
        {
            return  consumer.Create();
        }

        [HttpPost, ActionName(nameof(Create)), ValidateAntiForgeryToken]
        public Task<IActionResult> CreateConfirmed()
        {
            return consumer.CreateConfirmed();
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

        #region Delete
        public Task<IActionResult> Delete()
        {
            return consumer.Delete();
        }

        [HttpPost, ActionName(nameof(Delete)), ValidateAntiForgeryToken]
        public Task<IActionResult> DeleteFormData()
        {
            return consumer.DeleteConfirmed();
        }
        #endregion
     }
}