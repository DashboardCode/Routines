using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vse.Routines;
using Vse.Routines.Storage;
using Vse.AdminkaV1.Injected;
using Vse.AdminkaV1.Injected.Logging;

namespace Vse.AdminkaV1.Web
{
    public class MvcRoutine : AdminkaRoutine
    {
        public SessionState SessionState { get; private set; }
        public Controller Controller { get; private set; }
        public MvcRoutine(Controller controller, object input, [CallerMemberName] string action = "") :
            this(controller, WebManager.SetupCorrelationToken(controller.HttpContext), input, action)
        {

        }
        private MvcRoutine(Controller controller, Guid correlationToken, object input, string action) :
            this(controller,
                new RoutineTag(correlationToken, controller.GetType().Namespace, controller.GetType().Name, action),
                input)
        {
        }

        public MvcRoutine(Controller controller, RoutineTag routineTag, object input) :
            this(controller,
                 routineTag,
                 InjectedManager.NLogConstructor(
                       InjectedManager.Markdown,
                       InjectedManager.DefaultRoutineTagTransformException
                     ),
                 input)
        {
            controller.HttpContext.Items["routineTag"] = routineTag;
            var headers = controller.HttpContext.Response.Headers;
            if (headers["X-RoutineTag-CorrelationToken"].Count() == 0)
            {
                headers.Add("X-RoutineTag-CorrelationToken", routineTag.CorrelationToken.ToString());
                headers.Add("X-RoutineTag-Namespace", routineTag.Namespace);
                headers.Add("X-RoutineTag-Class", routineTag.Class);
                headers.Add("X-RoutineTag-Member", routineTag.Member);
            }
        }

        private MvcRoutine(Controller controller, RoutineTag routineTag,
            Func<RoutineTag, IResolver, RoutineLoggingTransients> loggingContainer,
            object input) :
            base(
                routineTag,
                controller.User.Identity,
                loggingContainer,
                new RepositoryHandlerFactory(InjectedManager.GetStorageMetaService()),
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
