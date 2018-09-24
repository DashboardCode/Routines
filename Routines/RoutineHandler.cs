using System;

namespace DashboardCode.Routines
{
    public class RoutineDisposeHandler<TClosure, TUserContext> : DisposeHandler<TClosure, RoutineClosure<TUserContext>> where TClosure : IDisposable
    {
        public RoutineDisposeHandler(
                Func<TClosure> createResource,
                RoutineClosure<TUserContext> closure
            ):base(createResource, closure)
        {
        }
    }

    public class RoutineDisposeHandler<TClosure, TUserContext, TDerivedClosure> : DisposeHandler<TClosure, RoutineClosure<TUserContext>, TDerivedClosure> 
        where TDerivedClosure : IDisposable, TClosure
    {
        public RoutineDisposeHandler(
                Func<TDerivedClosure> createResource,
                RoutineClosure<TUserContext> closure
            ) : base(createResource, closure)
        {
        }
    }

    public class RoutineHandler<TClosure, TUserContext> : Handler<TClosure, RoutineClosure<TUserContext>>
    {
        public RoutineHandler(
                Func<TClosure> createResource,
                RoutineClosure<TUserContext> closure
            ) : base(createResource, closure)
        {
        }
    }
}
