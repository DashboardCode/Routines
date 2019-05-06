//using System;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Mvc;
//using DashboardCode.Routines.Configuration;

//namespace DashboardCode.Routines.AspNetCore
//{
//    public class ApiRoutineHandler<TServiceContainer, TUserContext, TUser>
//    {
//        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
//        readonly Func<TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;
//        readonly Func<Exception, TUser, IActionResult> getErrorActionResult;

//        public ApiRoutineHandler(
//            ControllerBase controllerBase,
//            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
//            Func<AspRoutineFeature, Func<object>, TUser, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler,
//            Func<AspRoutineFeature, Exception, TUser, IActionResult> getErrorActionResult
//            )
//        {
//            var r = AspNetCoreManager.GetAspRoutineFeature(controllerBase);
//            Func<object> o = () => AspNetCoreManager.GetRequest(controllerBase.HttpContext.Request);
//            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(r);
//            this.getContainerHandler = (u, cf) => getContainerHandler(r, o, u, cf);
//            this.getErrorActionResult = (ex, u) => getErrorActionResult(r, ex, u);
//        }

//        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getContainerHandler(user, containerFactory);
//                var actionResult = await handler.HandleAsync((container, closure) => func(container, closure));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }

//        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = getUserAndFailedActionResultInitialisedAsync().Result;
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getContainerHandler(user, containerFactory);
//                var actionResult = handler.Handle((container, closure) => func(container, closure));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }

//        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, Task<IActionResult>> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getContainerHandler(user, containerFactory);
//                var actionResult = await handler.HandleAsync((container, closure) => func(container, closure, user));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }

//        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getContainerHandler(user, containerFactory);
//                var actionResult = handler.Handle((container, closure) => func(container, closure));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }

//        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, IActionResult> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getContainerHandler(user, containerFactory);
//                var actionResult = handler.Handle((container, closure) => func(container, closure, user));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }
//    }

//    public class ApiUserRoutineHandler<TUser, TUserContext>
//    {
//        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
//        readonly Func<TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler;
//        readonly Func<Exception, TUser, IActionResult> getErrorActionResult;

//        public ApiUserRoutineHandler(
//            ControllerBase controllerBase,
//            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
//            Func<AspRoutineFeature, TUser, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler,
//            Func<AspRoutineFeature, Exception, TUser, IActionResult> getErrorActionResult)
//        {
//            var r = AspNetCoreManager.GetAspRoutineFeature(controllerBase);
//            var o = AspNetCoreManager.GetRequest(controllerBase.HttpContext.Request);
//            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(r);
//            this.getUserHandler = (u, cf) => getUserHandler(r, u, cf);
//            this.getErrorActionResult = (ex, u) => getErrorActionResult(r, ex, u);
//        }

//        #region HandleUserAsync
//        public async Task<IActionResult> HandleUserAsync(Func<TUser, RoutineClosure<TUserContext>, IActionResult> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getUserHandler(user, containerFactory);
//                var actionResult = handler.Handle((u, closure) => func(u, closure));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }

//        public async Task<IActionResult> HandleUserAsync(Func<TUser, RoutineClosure<TUserContext>, Task<IActionResult>> func)
//        {
//            TUser internalUser = default;
//            try
//            {
//                var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
//                internalUser = user;
//                if (forbiddenActionResult != null)
//                    return forbiddenActionResult;

//                var handler = getUserHandler(user, containerFactory);
//                var actionResult = await handler.HandleAsync((u, closure) => func(u, closure));
//                return actionResult;
//            }
//            catch (Exception ex)
//            {
//                return getErrorActionResult(ex, internalUser);
//            }
//        }
//        #endregion
//    }

//    public class ApiRoutineAnonymousHandler<TServiceContainer, TUserContext>
//    {
//        readonly Func<ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

//        public ApiRoutineAnonymousHandler(ControllerBase controllerBase, Func<AspRoutineFeature, Func<object>, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler)
//        {
//            this.getContainerHandler = () => getContainerHandler(
//                AspNetCoreManager.GetAspRoutineFeature(controllerBase),
//                () => AspNetCoreManager.GetRequest(controllerBase.HttpContext.Request));
//        }

//        public IActionResult Handle<IActionResult>(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
//        {
//            return getContainerHandler()
//                .Handle((container, closure) => func(container, closure));
//        }

//        public Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
//        {
//            return getContainerHandler()
//                .HandleAsync((container, closure) => func(container, closure));
//        }
//    }
//}
