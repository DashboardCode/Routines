using System;
using System.Threading.Tasks;

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

    public class RoutineDisposeHandlerAsync<TClosure, TUserContext> : DisposeHandlerAsync<TClosure, RoutineClosure<TUserContext>> where TClosure : IDisposable
    {
        public RoutineDisposeHandlerAsync(
                Func<Task<TClosure>> createResource,
                RoutineClosure<TUserContext> closure
            ) : base(createResource, closure)
        {
        }
    }

    public class RoutineDisposeHandlerAsync<TClosure, TUserContext, TDerivedClosure> : DisposeHandlerAsync<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>
        where TDerivedClosure : IDisposable, TClosure
    {
        public RoutineDisposeHandlerAsync(
                Func<Task<TDerivedClosure>> createResource,
                RoutineClosure<TUserContext> closure
            ) : base(createResource, closure)
        {
        }
    }

    public class RoutineHandlerAsync<TClosure, TUserContext> : HandlerAsync<TClosure, RoutineClosure<TUserContext>>
    {
        public RoutineHandlerAsync(
                Func<Task<TClosure>> createResource,
                RoutineClosure<TUserContext> closure
            ) : base(createResource, closure)
        {
        }
    }
}
