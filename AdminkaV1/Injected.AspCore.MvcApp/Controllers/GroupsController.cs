using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Collections.Generic;
using DashboardCode.Routines.Configuration.Standard;
using Microsoft.Extensions.Options;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    //public static class Injected{
    //    public static Func<Expression<Func<TEntity, TProperty>>, Func<TEntity, Func<StringValues, VerboseListResult>>> MakeStraightAction<TEntity, TProperty>()
    //    {
    //        return null;
    //    }
    //}

    public class GroupsController : ConfigurableController
    {

        CrudRoutineControllerConsumer<Group, int> consumer;
        public GroupsController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) :base(applicationSettings, routineResolvablesOption.Value)
        {
            consumer = new CrudRoutineControllerConsumer<Group, int>(this, Meta.GroupMeta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
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
            return consumer.Create();
        }

        [HttpPost, ActionName(nameof(Create)), ValidateAntiForgeryToken]
        public Task<IActionResult> CreateFormData()
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