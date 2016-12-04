using System;
using Vse.AdminkaV1.Injected;
using Vse.Routines;

namespace Vse.AdminkaV1.Wcf
{
    public class WcfRoutine : AdminkaRoutine
    {
        public WcfRoutine(RoutineTag routineTag, string faultCodeNamespace, object input) 
            : base(routineTag, GetUserContext(), TransformException(faultCodeNamespace),  input)
        {
        }
        private static UserContext GetUserContext()
        {
            return new UserContext("Anonymous");
        }
        public static Func<Exception, RoutineTag, Func<Exception, string>, Exception> TransformException(string faultCodeNamespace)
        {
            return (ex,w,s)=>WcfException.TransformException(ex, w, faultCodeNamespace, s);
        }
    }
}