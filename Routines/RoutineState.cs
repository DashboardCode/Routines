using System;

namespace DashboardCode.Routines
{
    public class RoutineState<TUserContext>
    {
        private readonly IResolver resolver;
        public RoutineState(TUserContext userContext, RoutineTag routineTag, Action<DateTime, string> verbose, IResolver resolver)
        {
            UserContext = userContext;
            RoutineTag = routineTag;
            this.resolver = resolver;
            if (verbose != null)
                this.Verbose = (message) => verbose(DateTime.Now, message);
        }
        public Action<string> Verbose { get; private set; }
        public T Resolve<T>() where T : new()
        {
            return resolver.Resolve<T>();
        }
        public RoutineTag RoutineTag { get; private set; }
        public TUserContext UserContext { get; private set; }
    }
}
