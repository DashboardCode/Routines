using System.Runtime.CompilerServices;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.AuthenticationDom;
using System;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class PageRoutineHandler : PageRoutineHandler<PerCallContainer<UserContext>, UserContext, User>
    {
        public PageRoutineHandler(
            PageModel pageModel,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                pageModel,
                onPageRoutineFeature,
                defaultBackwardUrl,
                applicationSettings,
                memoryCache,
                new MemberTag(pageModel.GetType().Namespace, pageModel.GetType().Name, member)
                )
        {
        }

        private PageRoutineHandler(
            PageModel pageModel,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag) :
            base(
                pageModel, 
                ("BackwardUrl", defaultBackwardUrl, useReferer: true),
                (aspRoutineFeature) =>
                    MvcAppManager.GetUserAndFailedActionResultInitialisedAsync(
                        applicationSettings, 
                        memberTag, 
                        pageModel, 
                        aspRoutineFeature, 
                        memoryCache),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    {
                        var userContext = new UserContext(user);
                        pageModel.ViewData["UserContext"] = userContext;
                        return MvcAppManager.GetContainerHandler(
                            containerFactory,
                            memberTag,
                            aspRoutineFeature,
                            getInput,
                            userContext, 
                            applicationSettings,
                            uc => uc.AuditStamp,
                            uc => uc.AuditStamp
                        );
                    },
                onPageRoutineFeature
                )
        {
        }
    }

    public class PageRoutineAnonymousHandler : PageRoutineAnonymousHandler<PerCallContainer<AnonymousUserContext>, AnonymousUserContext>
    {
        public PageRoutineAnonymousHandler(
            ApplicationSettings applicationSettings, 
            PageModel pageModel,
            AnonymousUserContext anonymousUserContext,
            Action<PageRoutineFeature> onPageRoutineFeature,
            string defaultBackwardUrl = null,
            bool logRequest = false,
            [CallerMemberName] string member = null)
            : base(pageModel, 
                  ("BackwardUrl", defaultBackwardUrl, useReferer: true), 
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
                  }, onPageRoutineFeature

                  )
        {
        }
    }
}