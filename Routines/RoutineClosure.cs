using System;

namespace DashboardCode.Routines
{
    public class RoutineClosure<TUserContext>
    {
        public readonly Action<string> Verbose;
        public readonly TUserContext UserContext;
        readonly IContainer container;

        public RoutineClosure(TUserContext userContext, Action<DateTime, string> verbose, IContainer container)
        {
            UserContext = userContext;
            this.container = container;
            if (verbose != null)
                Verbose = (message) => verbose(DateTime.Now, message);
        }

        public T Resolve<T>() where T : new() => container.Resolve<T>();
    }
}