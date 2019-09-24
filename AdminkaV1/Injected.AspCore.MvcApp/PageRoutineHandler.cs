using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class PageContainerRoutineHandler : PageContainerRoutineHandler<UserContext, User>
    {
        public PageContainerRoutineHandler(
            PageModel pageModel,
            PageRoutineFeature pageRoutineFeature,
            [CallerMemberName] string member = null) : this(
                pageModel,
                pageRoutineFeature,
                (ApplicationSettings)pageModel.HttpContext.RequestServices.GetService(typeof(ApplicationSettings)),
                (IMemoryCache)pageModel.HttpContext.RequestServices.GetService(typeof(IMemoryCache)),
                new MemberTag(pageModel.GetType().Namespace, pageModel.GetType().Name, member),
                u=> new UserContext(u)
                )
        {
        }

        public PageContainerRoutineHandler(
            PageModel pageModel,
            PageRoutineFeature pageRoutineFeature,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag,
            Func<User, UserContext> createUserContext
            ) : base(
                    pageModel,
                    applicationSettings,
                    memberTag,
                    uc => uc.AuditStamp,
                    uc => uc.AuditStamp,
                    (aspRoutineFeature) =>
                     MvcAppManager.GetUserAndFailedActionResultInitialisedAsync(
                         applicationSettings,
                         memberTag,
                         pageModel,
                         aspRoutineFeature,
                         memoryCache,
                         pageRoutineFeature),
                    createUserContext
                )
        {
        }
    }

    public class PageContainerRoutineHandler<TUserContext, TUser> : PageRoutineHandler<PerCallContainer<TUserContext>, TUserContext, TUser>
    {
        public PageContainerRoutineHandler(
            PageModel pageModel,
            ApplicationSettings applicationSettings,
            MemberTag memberTag,
            Func<TUserContext, string> getConfigurationFor,
            Func<TUserContext, string> getAuditStamp,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> 
                getUserAndFailedActionResultInitialisedAsync,
            Func<TUser, TUserContext> createUnitContext
            ) : base(
                pageModel,
                getUserAndFailedActionResultInitialisedAsync,
                // TODO remove containerFactory
                (aspRoutineFeature, getInput, user, containerFactory) =>
                {
                    var userContext = createUnitContext(user);
                    return MvcAppManager.GetContainerHandler(
                        //containerFactory,
                        memberTag,
                        aspRoutineFeature,
                        getInput,
                        userContext,
                        applicationSettings,
                        getConfigurationFor,
                        getAuditStamp
                    );
                }
            )
        {
        }
    }

    public class PageStorageRoutineHandler<TUserContext, TUser> : PageRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext, TUser>
    {
        public PageStorageRoutineHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser,  ContainerFactory, ComplexRoutineHandler<StorageRoutineHandler<TUserContext>, TUserContext>> getContainerHandler
            ) : base(
                pageModel,
                getUserAndFailedActionResultInitialisedAsync,
                getContainerHandler
            )
        {
        }
    }

    public class PageRoutineAnonymousHandler : PageRoutineAnonymousHandler<PerCallContainer<AnonymousUserContext>, AnonymousUserContext>
    {
        public PageRoutineAnonymousHandler(
            PageModel pageModel,

            AnonymousUserContext anonymousUserContext,
            ApplicationSettings applicationSettings,
            bool logRequest = false,
            [CallerMemberName] string member = null)
            : base(     
                      pageModel,
                      (aspRoutineFeature, getInput) => {
                           var input = logRequest ? getInput() : default;
                           return
                               new AdminkaAnonymousRoutineHandler(
                                   applicationSettings,
                                   anonymousUserContext: anonymousUserContext,
                                   input: input,
                                   correlationToken: aspRoutineFeature.CorrelationToken,
                                   documentBuilder: aspRoutineFeature.TraceDocument.Builder,
                                   controllerNamespace: pageModel.GetType().Namespace,
                                   controllerName: pageModel.GetType().Name,
                                   member: member
                               );
                      }
                  )
        {
        }
    }
}