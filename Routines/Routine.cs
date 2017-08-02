using System;

namespace DashboardCode.Routines
{
    public class Routine<TUserContext>
    {
        private readonly IResolver resolver;
        public Routine(TUserContext userContext, RoutineGuid routineGuid, Action<DateTime, string> verbose, IResolver resolver)
        {
            UserContext = userContext;
            RoutineGuid = routineGuid;
            this.resolver = resolver;
            if (verbose != null)
                Verbose = (message) => verbose(DateTime.Now, message);
        }
        public T Resolve<T>() where T : new() => resolver.Resolve<T>();
        public Action<string> Verbose   { get; private set; }
        public RoutineGuid RoutineGuid    { get; private set; }
        public TUserContext UserContext { get; private set; }
    }
}
