using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.Configuration;

namespace DashboardCode.Routines.AspNetCore
{
    public class PageRoutineHandler<TServiceContainer, TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);
            this.getContainerHandler = (user, containerFactory) => getContainerHandler(aspRoutineFeature, getInput, user, containerFactory);
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

        //public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        //{
        //    var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
        //    if (forbiddenActionResult != null)
        //        return forbiddenActionResult;
        //    var handler = getContainerHandler(user, containerFactory);
        //    var actionResult = handler.Handle((container, closure) => func(container, closure));
        //    return actionResult;
        //}

        //public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, IActionResult> func)
        //{
        //    var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
        //    if (forbiddenActionResult != null)
        //        return forbiddenActionResult;
        //    var handler = getContainerHandler(user, containerFactory);
        //    var actionResult = handler.Handle((container, closure) => func(container, closure, user));
        //    return actionResult;
        //}
    }

    public class PageRoutineHandler<TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler;

        public PageRoutineHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser,   ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);

            this.getUserHandler = (user,  containerFactory) => getUserHandler(aspRoutineFeature, getInput, user, containerFactory);
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

        #endregion
    }

    public class PageRoutineAnonymousHandler<TServiceContainer, TUserContext>
    {
        readonly Func<ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineAnonymousHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler)
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            this.getContainerHandler = () => getContainerHandler(
                aspRoutineFeature,
                () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request));
        }


        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            return getContainerHandler()
                .Handle((container, closure) => func(container, closure));
        }

    }

    /*
    public class PageRoutineHandlerAsync2<TServiceContainer, TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, ComplexRoutineHandlerAsync2<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineHandlerAsync2(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandlerAsync2<TServiceContainer, TUserContext>> getContainerHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);
            this.getContainerHandler = (user, containerFactory) => getContainerHandler(aspRoutineFeature, getInput, user, containerFactory);
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


        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var handler = getContainerHandler(user, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure, user));
            return actionResult;
        }

    }
    */
    public class PageRoutineHandlerAsync<TServiceContainer, TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, ComplexRoutineHandlerAsync<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineHandlerAsync(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandlerAsync<TServiceContainer, TUserContext>> getContainerHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);
            this.getContainerHandler = (user, containerFactory) => getContainerHandler(aspRoutineFeature, getInput, user, containerFactory);
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


        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var handler = getContainerHandler(user, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure, user));
            return actionResult;
        }

    }
    public class PageRoutineHandlerAsync<TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, IRoutineHandlerAsync<TUser, TUserContext>> getUserHandler;

        public PageRoutineHandlerAsync(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, IRoutineHandlerAsync<TUser, TUserContext>> getUserHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);

            this.getUserHandler = (user, containerFactory) => getUserHandler(aspRoutineFeature, getInput, user, containerFactory);
        }

        #region HandleUserAsync
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
    /*
    public class PageRoutineAnonymousHandlerAsync2<TServiceContainer, TUserContext>
    {
        readonly Func<ComplexRoutineHandlerAsync2<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineAnonymousHandlerAsync2(
            PageModel pageModel,
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandlerAsync2<TServiceContainer, TUserContext>> getContainerHandler)
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            this.getContainerHandler = () => getContainerHandler(
                aspRoutineFeature,
                () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request));
        }

        public Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }


        public Task HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }
    }
    */
    public class PageRoutineAnonymousHandlerAsync<TServiceContainer, TUserContext>
    {
        readonly Func<ComplexRoutineHandlerAsync<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineAnonymousHandlerAsync(
            PageModel pageModel,
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandlerAsync<TServiceContainer, TUserContext>> getContainerHandler)
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            this.getContainerHandler = () => getContainerHandler(
                aspRoutineFeature,
                () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request));
        }

        public Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }


        public Task HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }
    }
}