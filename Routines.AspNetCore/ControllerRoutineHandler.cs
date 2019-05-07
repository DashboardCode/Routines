using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines.Configuration;

namespace DashboardCode.Routines.AspNetCore
{
    public class ControllerRoutineHandler<TServiceContainer, TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public ControllerRoutineHandler(
            ControllerBase controllerBase,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(controllerBase);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(controllerBase.HttpContext.Request);

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

    public class ControllerRoutineHandler<TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler;

        public ControllerRoutineHandler(
            ControllerBase controllerBase,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(controllerBase);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(controllerBase.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);
            this.getUserHandler = (user, containerFactory) => getUserHandler(aspRoutineFeature, getInput, user, containerFactory);
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

    public class ControllerRoutineAnonymousHandler<TServiceContainer, TUserContext>
    {
        readonly Func<ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public ControllerRoutineAnonymousHandler(
            ControllerBase controllerBase,
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler)
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(controllerBase);
            this.getContainerHandler = () => getContainerHandler(
                aspRoutineFeature,
                () => AspNetCoreManager.GetRequest(controllerBase.HttpContext.Request));
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