using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.Configuration;

namespace DashboardCode.Routines.AspNetCore
{
    public class PageRoutineHandler<TServiceContainer, TUserContext, TUser>
    {
        public PageRoutineFeature PageRoutineFeature { get; private set; }

        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineHandler(
            PageModel pageModel, 
            (string requestName, string defaultUrl, bool useReferer) backward,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler,
            Action<PageRoutineFeature> onPageRoutineFeature
            )
        {
            var r = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> o = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);
            PageRoutineFeature = AspNetCoreManager.GetPageRoutineFeature(pageModel.HttpContext.Request, backward);
            pageModel.HttpContext.Features.Set(PageRoutineFeature);
            onPageRoutineFeature?.Invoke(PageRoutineFeature);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(r);
            this.getContainerHandler = (u, cf) => getContainerHandler(r, o, u, cf);

        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getContainerHandler(user, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure));
            return actionResult;
        }

        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = getUserAndFailedActionResultInitialisedAsync().Result;
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getContainerHandler(user, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getContainerHandler(user, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure, user));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getContainerHandler(user, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getContainerHandler(user, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure, user));
            return actionResult;
        }
    }

    public class PageUserRoutineHandler<TUserContext, TUser>
    {
        public PageRoutineFeature PageRoutineFeature { get; private set; }

        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler;

        public PageUserRoutineHandler(
            PageModel pageModel, (string requestName, string defaultUrl, bool useReferer) backward,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler,
            Action<PageRoutineFeature> onPageRoutineFeature
            )
        {
            var r = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            var o = AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);
            PageRoutineFeature = AspNetCoreManager.GetPageRoutineFeature(pageModel.HttpContext.Request, backward);
            pageModel.HttpContext.Features.Set(PageRoutineFeature);
            onPageRoutineFeature?.Invoke(PageRoutineFeature);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(r);
            this.getUserHandler = (u, cf) => getUserHandler(r, u, cf);
        }

        #region HandleUserAsync
        public async Task<IActionResult> HandleAsync(Func<TUser, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getUserHandler(user, containerFactory);
            var actionResult = handler.Handle((u, closure) => func(u, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TUser, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;

            var handler = getUserHandler(user, containerFactory);
            var actionResult = await handler.HandleAsync((u, closure) => func(u, closure));
            return actionResult;
        }
        #endregion
    }

    public class PageRoutineAnonymousHandler<TServiceContainer, TUserContext>
    {
        public PageRoutineFeature PageRoutineFeature { get; set; }

        readonly Func<ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineAnonymousHandler(PageModel pageModel, (string requestName, string defaultUrl, bool useReferer) backward, 
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler,
            Action<PageRoutineFeature> onPageRoutineFeature)
        {
            PageRoutineFeature = AspNetCoreManager.GetPageRoutineFeature(pageModel.HttpContext.Request, backward);
            pageModel.HttpContext.Features.Set(PageRoutineFeature);
            onPageRoutineFeature?.Invoke(PageRoutineFeature);

            this.getContainerHandler = () => getContainerHandler(
                AspNetCoreManager.GetAspRoutineFeature(pageModel),
                () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request));

        }

        public Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }

        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            return getContainerHandler()
                .Handle((container, closure) => func(container, closure));
        }

        public Task HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }
    }
}