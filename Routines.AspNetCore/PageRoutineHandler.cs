﻿using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.Configuration;

namespace DashboardCode.Routines.AspNetCore
{
    public class PageRoutineHandler<TServiceContainer, TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUserContext, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;
        readonly Func<TUser, TUserContext> createUnitContext;

        public PageRoutineHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<TUser, TUserContext> createUnitContext,
            Func<AspRoutineFeature, Func<object>, TUserContext, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler
            )
        {
            var r = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> o = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(r);
            this.getContainerHandler = (u, cf) => getContainerHandler(r, o, u, cf);
            this.createUnitContext = createUnitContext;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(userContext, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure));
            return actionResult;
        }

        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = getUserAndFailedActionResultInitialisedAsync().Result;
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(userContext, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(userContext, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure, user));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(userContext, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(userContext, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure, user));
            return actionResult;
        }
    }

    public class PageUserRoutineHandler<TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, TUserContext> createUnitContext;
        readonly Func<TUserContext, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler;

        public PageUserRoutineHandler(
            PageModel pageModel,
            PageRoutineFeature pageRoutineFeature,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<TUser, TUserContext> createUnitContext,
            Func<AspRoutineFeature, TUserContext, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler
            )
        {
            var r = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            var o = AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);
            pageModel.HttpContext.Features.Set(pageRoutineFeature);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(r);
            this.createUnitContext = createUnitContext;
            this.getUserHandler = (u, cf) => getUserHandler(r, u, cf);
        }

        #region HandleUserAsync
        public async Task<IActionResult> HandleAsync(Func<TUser, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getUserHandler(userContext, containerFactory);
            var actionResult = handler.Handle((u, closure) => func(u, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TUser, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getUserHandler(userContext, containerFactory);
            var actionResult = await handler.HandleAsync((u, closure) => func(u, closure));
            return actionResult;
        }
        #endregion
    }

    public class PageRoutineAnonymousHandler<TServiceContainer, TUserContext>
    {
        readonly Func<ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineAnonymousHandler(
            PageModel pageModel,
            PageRoutineFeature pageRoutineFeature, 
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler)
        {
            this.getContainerHandler = () => getContainerHandler(
                AspNetCoreManager.GetAspRoutineFeature(pageModel),
                () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request));
            pageModel.HttpContext.Features.Set(pageRoutineFeature);
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