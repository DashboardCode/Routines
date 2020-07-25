using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class ComplexRoutineDisposeHandler<TClosure, TUserContext> : ComplexDisposeHandler<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandler(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ) : base (createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandler<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandler<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandler(
            Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            ):base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandler<TClosure, TUserContext> : ComplexHandler<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext>
    {
        public ComplexRoutineHandler(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ):base(createResource, handler)
        {
        }
    }

    // ---------------------------------    
    public class ComplexRoutineDisposeHandlerAsync<TClosure, TUserContext> :
            ComplexDisposeHandlerAsync<TClosure, RoutineClosure<TUserContext>>, IRoutineHandlerAsync<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandlerAsync(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandlerAsync<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandlerAsync<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandlerAsync<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandlerAsync<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandlerAsync(
            Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler
            ) : base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandlerAsync<TClosure, TUserContext> : ComplexHandlerAsync<TClosure, RoutineClosure<TUserContext>>, IRoutineHandlerAsync<TClosure, TUserContext>
    {
        public ComplexRoutineHandlerAsync(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandlerAsync<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }
    // ---------------------------------
    public class ComplexRoutineDisposeHandlerAsync2<TClosure, TUserContext> :
        ComplexDisposeHandlerAsync2<TClosure, RoutineClosure<TUserContext>>, IRoutineHandlerAsync<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandlerAsync2(
            Func<RoutineClosure<TUserContext>, Task<TClosure>> createResource,
            IHandlerAsync<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandlerAsync2<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandlerAsync2<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandlerAsync<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandlerAsync2(
            Func<RoutineClosure<TUserContext>, Task<TDerivedClosure>> createResource,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler
            ) : base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandlerAsync2<TClosure, TUserContext> : ComplexHandlerAsync2<TClosure, RoutineClosure<TUserContext>>, IRoutineHandlerAsync<TClosure, TUserContext>
    {
        public ComplexRoutineHandlerAsync2(
            Func<RoutineClosure<TUserContext>, Task<TClosure>> createResource,
            IHandlerAsync<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    // ---------------------------------

    public class ComplexRoutineDisposeHandlerOmni<TClosure, TUserContext> : ComplexDisposeHandlerOmni<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandlerOmni(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandlerOmni<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandlerOmni<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandlerOmni<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandlerOmni(
            Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource,
            IHandlerOmni<RoutineClosure<TUserContext>> routineHandler
            ) : base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandlerOmni<TClosure, TUserContext> : ComplexHandlerOmni<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext>
    {
        public ComplexRoutineHandlerOmni(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandlerOmni<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    // ---------------------------------

    public class ComplexRoutineDisposeHandlerOmni2<TClosure, TUserContext> : ComplexDisposeHandlerOmni2<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandlerOmni2(
            Func<RoutineClosure<TUserContext>, Task<TClosure>> createResource,
            IHandlerOmni<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandlerOmni2<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandlerOmni2<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandlerOmni2(
            Func<RoutineClosure<TUserContext>, Task<TDerivedClosure>> createResource,
            IHandlerOmni<RoutineClosure<TUserContext>> routineHandler
            ) : base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandlerOmni2<TClosure, TUserContext> : ComplexHandlerOmni2<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext>
    {
        public ComplexRoutineHandlerOmni2(
            Func<RoutineClosure<TUserContext>, Task<TClosure>> createResource,
            IHandlerOmni<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }


}
