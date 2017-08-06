using System;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Wcf.Messaging
{
    public class WcfRoutine : AdminkaRoutine
    {
        public WcfRoutine(MemberTag memberTag, string faultCodeNamespace, object input) 
            : this(new RoutineGuid(memberTag), GetUserContext(), faultCodeNamespace,
                  new WcfApplicationFactory(), input)
        {
        }

        protected WcfRoutine(RoutineGuid routineGuid, UserContext userContext, string faultCodeNamespace, 
            IApplicationFactory applicationFactory, object input)
            : base(routineGuid, userContext, TransformException(faultCodeNamespace),
                   applicationFactory, input)
        {
        }

        private static UserContext GetUserContext() =>
            new UserContext("Anonymous");

        public static Func<Exception, RoutineGuid, Func<Exception, string>, Exception> TransformException(string faultCodeNamespace) =>
            (ex,w,s)=>WcfException.TransformException(ex, w, faultCodeNamespace, s);
    }
}