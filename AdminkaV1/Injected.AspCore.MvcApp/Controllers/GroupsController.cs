using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;
using Microsoft.Extensions.Primitives;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Controllers
{
    public static class Injected{
        public static Func<Expression<Func<TEntity, TProperty>>, Func<TEntity, Func<StringValues, VerboseResult>>> MakeStraightAction<TEntity, TProperty>()
        {
            return null;
        }
    }

    public class GroupsController : RoutineController
    {
        #region Meta
        static ControllerMeta<Group, int> meta = new ControllerMeta<Group, int>(
            id => e => e.GroupId == id,
            Converters.TryParseInt,
            manyToMany => manyToMany.Add(
                "Privileges",
                repository => repository.Clone<Privilege>().List(),
                nameof(Privilege.PrivilegeName),
                e => e.GroupPrivilegeMap,
                mm => mm.PrivilegeId,
                mm => mm.GroupId,
                e => e.PrivilegeId,
                
                (ep, ef) => new GroupPrivilege() { GroupId = ep.GroupId, PrivilegeId = ef.PrivilegeId }
            
            ).Add("Roles",
                    repository => repository.Clone<Role>().List(),
                    nameof(Role.RoleName),
                    e => e.GroupRoleMap,
                    mm => mm.RoleId,
                    mm => mm.GroupId,
                    e => e.RoleId,
                    
                    (ep, ef) => new GroupRole() { GroupId = ep.GroupId, RoleId = ef.RoleId }
            ).Add(
                    "Users",
                    repository => repository.Clone<User>().List(),
                    nameof(AuthenticationDom.User.LoginName),
                    e => e.UserGroupMap,
                    mm => mm.UserId, 
                    mm => mm.GroupId,
                    e => e.UserId,
                    
                    (ep, ef) => new UserGroup() { GroupId = ep.GroupId, UserId = ef.UserId }
            ),
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
            //editables.Add(e => e.GroupName, sv => Binder.ConvertToString(sv), s=>Binder.ValidateStringLength(10) )
            formFields =>
                formFields.Add(e => e.GroupName,   Binder.ConvertToString)
                          .Add(e => e.GroupAdName, Binder.ConvertToString, asserts => asserts.Add(v => v.Length <= 18, "Too big!"))
                         //.Add(Injected.MakeEditable(e => e.GroupAdName) )
            //.Add(e => e.GroupAdName, setter => sv => Binder.TryStringValidateLength(sv, v => setter(v), 100))
            //.Add(e => e.GroupId,  Binder.ConvertToInt, asserts => asserts.Add(v => v < 100, "Too big!"))
            //.Add(e => e.GroupId,  Binder.ConvertToInt, v=>new BinderResult(v<100? null: "Too big!"), converter => setter => validator => validator(setter(converter())))
            //.Add(e => e.GroupId,  Binder.ConvertToInt,  convertor => setter => { var r = setter(convertor()); if (r.Value > 100) {; } return r.BinderResult; })
            //.Add(e => e.GroupId,  setter => sv => { if (!int.TryParse(sv.ToString(), out int v)) return new BinderResult("Can't parse"); setter(v);     return new BinderResult(v<100?null: "Too big!"); })
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


        //
        );
        #endregion

        CrudRoutineControllerConsumer<Group, int> consumer;
        public GroupsController(IConfigurationRoot  configurationRoot) :base(configurationRoot)
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
            return await consumer.CreateFormData();
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
            return await consumer.EditFormData();
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
            return await consumer.DeleteFormData();
        }
        #endregion
    }
}