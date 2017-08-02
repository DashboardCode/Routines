using System;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.Wcf.Messaging
{
    public class WcfRoutine : AdminkaRoutine
    {
        public WcfRoutine(MemberGuid routineTag, string faultCodeNamespace, object input) 
            : base(routineTag, GetUserContext(), TransformException(faultCodeNamespace), new ConfigurationNETFramework(), input)
        {
        }
        private static UserContext GetUserContext() =>
            new UserContext("Anonymous");

        public static Func<Exception, MemberGuid, Func<Exception, string>, Exception> TransformException(string faultCodeNamespace) =>
            (ex,w,s)=>WcfException.TransformException(ex, w, faultCodeNamespace, s);
    }
}