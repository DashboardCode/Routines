using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public interface IHandler<TClosure>
    {
        void Handle(Action<TClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TOutput> func);
        Task HandleAsync(Func<TClosure, Task> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func);
    }

    public interface IHandler<TClosure, TSuperClosure> : IHandler<TClosure>
    {
        void Handle(Action<TClosure, TSuperClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func);
        Task HandleAsync(Func<TClosure, TSuperClosure, Task> func);
    }
}