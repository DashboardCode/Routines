using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public interface IHandler<TClosure>
    {
        void Handle(Action<TClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TOutput> func);
    }

    public interface IHandler<TClosure, TSuperClosure> : IHandler<TClosure>
    {
        void Handle(Action<TClosure, TSuperClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TSuperClosure, TOutput> func);
    }


    public interface IHandlerAsync<TClosure>
    {
        Task HandleAsync(Func<TClosure, Task> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func);
    }

    public interface IHandlerOmni<TClosure>: IHandler<TClosure>, IHandlerAsync<TClosure>
    {
    }


    public interface IHandlerAsync<TClosure, TSuperClosure> : IHandlerAsync<TClosure>
    {
        Task HandleAsync(Func<TClosure, TSuperClosure, Task> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TSuperClosure, Task<TOutput>> func);
    }

    public interface IHandlerOmni<TClosure, TSuperClosure> : IHandler<TClosure, TSuperClosure>, IHandlerAsync<TClosure, TSuperClosure>
    {
    }
}