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
using DashboardCode.Routines.Configuration.NETStandard;
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
        #region Meta
        static ControllerMeta<Group, int> meta = new ControllerMeta<Group, int>(
            id => e => e.GroupId == id,
            Converters.TryParseInt,
            chain => chain
                .IncludeAll(e => e.GroupPrivilegeMap)
                .ThenInclude(e => e.Privilege)
                .IncludeAll(e => e.GroupRoleMap)
                .ThenInclude(e => e.Role)
                .IncludeAll(e => e.UserGroupMap)
                .ThenInclude(e => e.User),
            chain => chain.IncludeAll(e => e.GroupPrivilegeMap)
                .ThenInclude(e => e.Privilege)
                .IncludeAll(e => e.GroupRoleMap)
                .ThenInclude(e => e.Role)
                .IncludeAll(e => e.UserGroupMap)
                .ThenInclude(e => e.User),
            null,
            //var roleNameLength = metaBrowser.GetLength<Role>(en => en.RoleName);
            formFields =>
                formFields.Add(e => e.GroupName,   Binder.ConvertToString)
                          .Add(e => e.GroupAdName, Binder.ConvertToString, asserts => asserts.Add(v => v.Length <= 18, "Too big!"))
                          //.Add(e => e.GroupAdName, setter => sv => Binder.TryStringValidateLength(sv, v => setter(v), 100))
                          //.Add(e => e.GroupId,  Binder.ConvertToInt, asserts => asserts.Add(v => v < 100, "Too big!"))
                          //.Add(e => e.GroupId,  Binder.ConvertToInt, v=>new BinderResult(v<100? null: new[] { "Too big!" }), converter => setter => validator => validator(setter(converter())))
                          //.Add(e => e.GroupId,  Binder.ConvertToInt,  convertor => setter => { var r = setter(convertor()); if (r.Value > 100) {; } return r.ToVerboseResult(); })
                          //.Add(e => e.GroupId,  setter => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); setter(v);    return new BinderResult(v<100?null: "Too big!"); })
                          //.Add("GroupId",           e => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); e.GroupId = v; return new BinderResult(v<100?null:"Too bug!"); })
            ,

            hiddenFormFields =>
                hiddenFormFields.Add(e => e.GroupId)
                            .Add(e => e.RowVersion)
                            //.Add(e => e.GroupId, sv => int.Parse(sv.ToString()))
                            //.Add(e => e.GroupId, sv => int.Parse(sv.ToString()), convertor => setter => setter(convertor()))
                            //.Add(e => e.GroupId, setter => sv => setter(int.Parse(sv.ToString())))
                            //.Add("GroupId", e => e.GroupId, setter => sv => setter(int.Parse(sv.ToString())))
                            //.Add("GroupId", e => sv => e.GroupId = int.Parse(sv.ToString()))
            ,
            null,
            manyToMany => manyToMany.Add(
                "Privileges",
                "PrivilegesMultiSelectList",
                repository => repository.Clone<Privilege>().List(),
                e => e.GroupPrivilegeMap,
                mm => mm.PrivilegeId,
                mm => mm.GroupId,
                e => e.PrivilegeId,
                nameof(Privilege.PrivilegeId),
                nameof(Privilege.PrivilegeName),
                (ep, ef) => new GroupPrivilege() { GroupId = ep.GroupId, PrivilegeId = ef.PrivilegeId }
            ).Add(
                "Roles",
                "RolesMultiSelectList",
                repository => repository.Clone<Role>().List(),
                e => e.GroupRoleMap,
                mm => mm.RoleId,
                mm => mm.GroupId,
                e => e.RoleId,
                nameof(Role.RoleId),
                nameof(Role.RoleName),
                (ep, ef) => new GroupRole() { GroupId = ep.GroupId, RoleId = ef.RoleId }
            ).Add(
                "Users",
                "UsersMultiSelectList",
                repository => repository.Clone<User>().List(),
                e => e.UserGroupMap,
                mm => mm.UserId,
                mm => mm.GroupId,
                e => e.UserId,
                nameof(AuthenticationDom.User.UserId),
                nameof(AuthenticationDom.User.LoginName),
                (ep, ef) => new UserGroup() { GroupId = ep.GroupId, UserId = ef.UserId }
            )


        );
        #endregion

        CrudRoutineControllerConsumer<Group, int> consumer;
        public GroupsController(ApplicationSettings applicationSettings, IOptionsSnapshot<List<RoutineResolvable>> routineResolvablesOption) :base(applicationSettings, routineResolvablesOption.Value)
        {
            consumer = new CrudRoutineControllerConsumer<Group, int>(this, meta, (action, userContext) => userContext.HasPrivilege(Privilege.ConfigureSystem));
        }

        #region Details / Index
        public async Task<IActionResult> Details()
        {
            return await consumer.Details();
        }

        public async Task<IActionResult> Index()
        {
            return await consumer.Index();
        }
        #endregion

        #region Create
        public async Task<IActionResult> Create()
        {
            return await consumer.Create();
        }

        [HttpPost, ActionName(nameof(Create)), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFormData()
        {
            return await consumer.CreateConfirmed();
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit()
        {
            return await consumer.Edit();
        }

        [HttpPost, ActionName(nameof(Edit)), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFormData()
        {
            return await consumer.EditConfirmed();
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete()
        {
            return await consumer.Delete();
        }

        [HttpPost, ActionName(nameof(Delete)), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFormData()
        {
            return await consumer.DeleteConfirmed();
        }
        #endregion
    }
}