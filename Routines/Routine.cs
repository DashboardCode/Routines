using System;

namespace DashboardCode.Routines
{
    public class Routine<TUserContext>
    {
        public readonly Action<string> Verbose;
        public readonly RoutineGuid RoutineGuid;
        public readonly TUserContext UserContext;
        readonly IContainer resolver;

        public Routine(TUserContext userContext, RoutineGuid routineGuid, Action<DateTime, string> verbose, IContainer resolver)
        {
            UserContext = userContext;
            RoutineGuid = routineGuid;
            this.resolver = resolver;
            if (verbose != null)
                Verbose = (message) => verbose(DateTime.Now, message);
        }

        public T Resolve<T>() where T : new() => resolver.Resolve<T>();
    }
}