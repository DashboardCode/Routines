using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using DashboardCode.Routines;
using DashboardCode.Routines.AspNetCore;

using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore
{
    //  TODO (aspRoutineFeature, ex, internalUser) => AspCoreManager.GetErrorActionResult(ex, aspRoutineFeature.AspRequestId, applicationSettings.ForceDetailsOnCustomErrorPage, internalUser)
    public class ApiRoutineHandler : ControllerRoutineHandler<PerCallContainer<UserContext>, UserContext, User>
    {
        public ApiRoutineHandler(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                controllerBase,
                applicationSettings,
                memoryCache,
                new MemberTag(controllerBase.GetType().Namespace, controllerBase.GetType().Name, member)
             )
        {

        }

        public ApiRoutineHandler(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag
            ) : base(controllerBase,
                (aspRoutineFeature) => AspNetCoreManager.GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag, controllerBase,
                    aspRoutineFeature, memoryCache, aspRoutineFeature.AspRequestId),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    AspNetCoreManager.GetContainerHandler(
                        aspRoutineFeature,
                        getInput,
                        user,
                        new UserContext(user),
                        containerFactory,
                        memberTag,
                        applicationSettings,
                        uc => uc.User.LoginName,
                        uc => uc.User.LoginName
                    )
             )
        {

        }


    }

    public class ApiUserRoutineHandler : ControllerRoutineHandler<UserContext, User>
    {
        public ApiUserRoutineHandler(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                controllerBase,
                applicationSettings,
                memoryCache,
                new MemberTag(controllerBase.GetType().Namespace, controllerBase.GetType().Name, member)
             )
        {
        }

        public ApiUserRoutineHandler(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag
            ) : base(
                controllerBase,
                (aspRoutineFeature) => AspNetCoreManager.GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag, controllerBase, aspRoutineFeature, memoryCache, aspRoutineFeature.AspRequestId),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    AspNetCoreManager.GetUserHandler(
                        aspRoutineFeature,
                        getInput,
                        user,
                        new UserContext(user),
                        containerFactory,
                        memberTag,
                        applicationSettings,
                        uc => uc.User.LoginName
                    )
             )
        {

        }
    }

    // ----------
    /*
    public class ApiRoutineHandlerAsync2 : ControllerRoutineHandlerAsync2<PerCallContainer<UserContext>, UserContext, User>
    {
        public ApiRoutineHandlerAsync2(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                controllerBase,
                applicationSettings,
                memoryCache,
                new MemberTag(controllerBase.GetType().Namespace, controllerBase.GetType().Name, member)
             )
        {

        }

        public ApiRoutineHandlerAsync2(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag
            ) : base(controllerBase,
                (aspRoutineFeature) => AspNetCoreManager.GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag, controllerBase,
                    aspRoutineFeature, memoryCache, aspRoutineFeature.AspRequestId),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    AspNetCoreManager.GetContainerHandlerAsync2(
                        aspRoutineFeature,
                        getInput,
                        user,
                        new UserContext(user),
                        containerFactory,
                        memberTag,
                        applicationSettings,
                        uc => uc.User.LoginName,
                        uc => uc.User.LoginName
                    )
             )
        {

        }


    }

    public class ApiUserRoutineHandlerAsync2 : ControllerRoutineHandlerAsync2<UserContext, User>
    {
        public ApiUserRoutineHandlerAsync2(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                controllerBase,
                applicationSettings,
                memoryCache,
                new MemberTag(controllerBase.GetType().Namespace, controllerBase.GetType().Name, member)
             )
        {
        }

        public ApiUserRoutineHandlerAsync2(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag
            ) : base(
                controllerBase,
                (aspRoutineFeature) => AspNetCoreManager.GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag, controllerBase, aspRoutineFeature, memoryCache, aspRoutineFeature.AspRequestId),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    AspNetCoreManager.GetUserHandlerAsync(
                        aspRoutineFeature,
                        getInput,
                        user,
                        new UserContext(user),
                        containerFactory,
                        memberTag,
                        applicationSettings,
                        uc => uc.User.LoginName
                    )
             )
        {

        }
    }
    */
    public class ApiRoutineHandlerAsync : ControllerRoutineHandlerAsync<PerCallContainer<UserContext>, UserContext, User>
    {
        public ApiRoutineHandlerAsync(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                controllerBase,
                applicationSettings,
                memoryCache,
                new MemberTag(controllerBase.GetType().Namespace, controllerBase.GetType().Name, member)
             )
        {

        }

        public ApiRoutineHandlerAsync(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag
            ) : base(controllerBase,
                (aspRoutineFeature) => AspNetCoreManager.GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag, controllerBase,
                    aspRoutineFeature, memoryCache, aspRoutineFeature.AspRequestId),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    AspNetCoreManager.GetContainerHandlerAsync(
                        aspRoutineFeature,
                        getInput,
                        user,
                        new UserContext(user),
                        containerFactory,
                        memberTag,
                        applicationSettings,
                        uc => uc.User.LoginName,
                        uc => uc.User.LoginName
                    )
             )
        {

        }


    }
    public class ApiUserRoutineHandlerAsync : ControllerRoutineHandlerAsync<UserContext, User>
    {
        public ApiUserRoutineHandlerAsync(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            [CallerMemberName] string member = null) : this(
                controllerBase,
                applicationSettings,
                memoryCache,
                new MemberTag(controllerBase.GetType().Namespace, controllerBase.GetType().Name, member)
             )
        {
        }

        public ApiUserRoutineHandlerAsync(
            ControllerBase controllerBase,
            ApplicationSettings applicationSettings,
            IMemoryCache memoryCache,
            MemberTag memberTag
            ) : base(
                controllerBase,
                (aspRoutineFeature) => AspNetCoreManager.GetUserAndFailedActionResultInitialisedAsync(applicationSettings, memberTag, controllerBase, aspRoutineFeature, memoryCache, aspRoutineFeature.AspRequestId),
                (aspRoutineFeature, getInput, user, containerFactory) =>
                    AspNetCoreManager.GetUserHandlerAsync(
                        aspRoutineFeature,
                        getInput,
                        user,
                        new UserContext(user),
                        containerFactory,
                        memberTag,
                        applicationSettings,
                        uc => uc.User.LoginName
                    )
             )
        {

        }
    }
}