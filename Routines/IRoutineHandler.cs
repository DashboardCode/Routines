using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public interface IRoutineHandler<TClosure, TUserContext> : IHandler<TClosure>
    {
        void Handle(Action<TClosure, RoutineClosure<TUserContext>> action);
        TOutput Handle<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, RoutineClosure<TUserContext>, Task<TOutput>> func);
        Task HandleAsync(Func<TClosure, RoutineClosure<TUserContext>, Task> func);
    }
}