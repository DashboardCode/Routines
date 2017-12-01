using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.AdminkaV1.Injected.Logging;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class MvcRoutine : AdminkaRoutineHandler
    {
        public readonly SessionState SessionState;
        public readonly RoutineController Controller;
        public MvcRoutine(RoutineController controller, object input, [CallerMemberName] string action = "") :
            this(controller, WebManager.SetupCorrelationToken(controller.HttpContext), input, action)
        {
        }
        private MvcRoutine(RoutineController controller, Guid correlationToken, object input, string action) :
            this(controller,
                new RoutineGuid(correlationToken, controller.GetType().Namespace, controller.GetType().Name, action),
                input)
        {
        }
        public MvcRoutine(RoutineController controller, RoutineGuid routineGuid, object input) :
            this(controller,
                 routineGuid,
                 InjectedManager.ComposeNLogTransients(
                       InjectedManager.Markdown,
                       InjectedManager.DefaultRoutineTagTransformException
                     ),
                 new MvcApplicationFactory(controller.ConfigurationRoot),
                 input)
        {
            controller.HttpContext.Items["routineGuid"] = routineGuid;
            var headers = controller.HttpContext.Response.Headers;
            if (headers["X-RoutineGuid-CorrelationToken"].Count() == 0)
            {
                headers.Add("X-RoutineGuid-CorrelationToken",    routineGuid.CorrelationToken.ToString());
                headers.Add("X-RoutineGuid-MemberTag-Namespace", routineGuid.MemberTag.Namespace);
                headers.Add("X-RoutineGuid-MemberTag-Type",      routineGuid.MemberTag.Type);
                headers.Add("X-RoutineGuid-MemberTag-Member",    routineGuid.MemberTag.Member);
            }
        }
        private MvcRoutine(RoutineController controller, RoutineGuid routineGuid,
            Func<RoutineGuid, IContainer, RoutineLoggingTransients> loggingFactory,
            IApplicationFactory applicationFactory,
            object input) :
            base(
                routineGuid,
                controller.User.Identity,
                loggingFactory,
                applicationFactory,
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