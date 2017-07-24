using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public class MvcRoutine : AdminkaRoutine
    {
        public SessionState SessionState { get; private set; }
        public RoutineController Controller { get; private set; }
        public MvcRoutine(RoutineController controller, object input, [CallerMemberName] string action = "") :
            this(controller, WebManager.SetupCorrelationToken(controller.HttpContext), input, action)
        {
        }
        private MvcRoutine(RoutineController controller, Guid correlationToken, object input, string action) :
            this(controller,
                new RoutineTag(correlationToken, controller.GetType().Namespace, controller.GetType().Name, action),
                input)
        {
        }
        public MvcRoutine(RoutineController controller, RoutineTag routineTag, object input) :
            this(controller,
                 routineTag,
                 InjectedManager.ComposeNLogTransients(
                       InjectedManager.Markdown,
                       InjectedManager.DefaultRoutineTagTransformException
                     ),
                 new MvcAppConfiguration(controller.ConfigurationRoot),
                 input)
        {
            controller.HttpContext.Items["routineTag"] = routineTag;
            var headers = controller.HttpContext.Response.Headers;
            if (headers["X-RoutineTag-CorrelationToken"].Count() == 0)
            {
                headers.Add("X-RoutineTag-CorrelationToken", routineTag.CorrelationToken.ToString());
                headers.Add("X-RoutineTag-Namespace", routineTag.Namespace);
                headers.Add("X-RoutineTag-Type", routineTag.Type);
                headers.Add("X-RoutineTag-Member", routineTag.Member);
            }
        }
        private MvcRoutine(RoutineController controller, RoutineTag routineTag,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingContainer,
            MvcAppConfiguration appConfiguration,
            object input) :
            base(
                routineTag,
                controller.User.Identity,
                loggingContainer,
                new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService(appConfiguration)),
                appConfiguration,
                input)
        {
            this.Controller = controller;
            this.SessionState = new SessionState(controller.HttpContext.Session, userContext);
            controller.ViewBag.Session = this.SessionState;
            controller.ViewBag.UserContext = userContext;
        }
    }

    public class SessionState
    {
        ISession session;
        UserContext userContext;
        public SessionState(ISession session, UserContext userContext)
        {
            this.session = session;
            this.userContext = userContext;
        }
        public string UserContextKey
        {
            get
            {
                return session.GetString("UserContextKey");
            }
            set
            {
                session.SetString("UserContextKey", value);
            }
        }
    }
}
