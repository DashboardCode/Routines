using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.AuthenticationDom;
using Microsoft.AspNetCore.Http;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class AdminkaReferrer : Referrer
    {
        public AdminkaReferrer(HttpRequest request, string defaultReferrer, Func<string> getId,
            string entityName ) : base(
            AspNetCoreManager.GetReferrer(request, "Referrer", defaultReferrer, false), getId, entityName, "Referrer"
            ) 
        {

        }
    }
    // TODO reload configuration on changed
    // var trackedConfigurationSnapshot = (IOptionsSnapshot<List<RoutineResolvable>>)pageModel.HttpContext.RequestServices.GetService(typeof(IOptionsSnapshot<List<RoutineResolvable>>));
    // var trackedConfiguration = trackedConfigurationSnapshot.Value;
    public class AdminkaCrudRoutinePageConsumer<TEntity, TKey> : AdminkaCrudRoutinePageConsumer<UserContext, User, TEntity, TKey> where TEntity : class, new()
    {
        public readonly Referrer Referrer;

        public AdminkaCrudRoutinePageConsumer(
            PageModel pageModel,
            string defaultReferrer,
            [CallerMemberName] string member = null) :
            this(pageModel,
                 new AdminkaReferrer(
                     pageModel.Request,
                     defaultReferrer, 
                     null,
                     null
                 ), 
                 member
            )
        {
        }

        public AdminkaCrudRoutinePageConsumer(
            PageModel pageModel, Referrer referrer,
            [CallerMemberName] string member = null) :
            this(pageModel, referrer,
                MvcAppManager.SetAndGetPageRoutineFeature(pageModel, referrer),
                u => MvcAppManager.SetAndGetUserContext(pageModel, u),
                (ApplicationSettings)pageModel.HttpContext.RequestServices.GetService(typeof(ApplicationSettings)),
                (IMemoryCache)pageModel.HttpContext.RequestServices.GetService(typeof(IMemoryCache)),
                new MemberTag(pageModel.GetType().Namespace, pageModel.GetType().Name, member))
        {
        }

        public AdminkaCrudRoutinePageConsumer(PageModel pageModel, Referrer referrer,
             PageRoutineFeature pageRoutineFeature,
             Func<User, UserContext> createUserContext, 
             ApplicationSettings applicationSettings, IMemoryCache memoryCache, MemberTag memberTag) :
             base(pageModel, pageRoutineFeature,
                 getUserAndFailedActionResultInitialisedAsync:
                 (aspRoutineFeature)=>
                     MvcAppManager.GetUserAndFailedActionResultInitialisedAsync(
                         applicationSettings,
                         memberTag,
                         pageModel,
                         aspRoutineFeature,
                         memoryCache,
                         pageRoutineFeature),
                getContainerHandler: (aspRoutineFeature, getInput, user, containerFactory) =>
                {
                    var userContext = createUserContext(user);
                    return MvcAppManager.GetContainerStorageHandler(
                        containerFactory,
                        memberTag,
                        aspRoutineFeature,
                        getInput,
                        userContext,
                        applicationSettings,
                        uc => uc.AuditStamp
                    );
                })
        {
            this.Referrer = referrer;
        }
    }

    public class AdminkaCrudRoutinePageConsumer<TUserContext, TUser, TEntity, TKey> : CrudRoutinePageConsumer<TUserContext, TUser, TEntity, TKey> where TEntity : class, new()
    {
        public AdminkaCrudRoutinePageConsumer(
            PageModel pageModel, 
            PageRoutineFeature pageRoutineFeature,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>> getContainerHandler) :
            base(
                pageModel,
                pageRoutineFeature,
                new PageStorageRoutineHandler<TUserContext, TUser>(
                    pageModel,
                    getUserAndFailedActionResultInitialisedAsync,
                    getContainerHandler
                    )
                )
        {
        }
    }
}