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
}