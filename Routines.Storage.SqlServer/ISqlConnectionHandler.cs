using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage.SqlServer
{
    //public interface IResourceHandler<TUserContext, TResource>
    //{
    //    void Handle(Action<TResource> action);
    //    TOutput Handle<TOutput>(Func<TResource, TOutput> func);
    //    Task<TOutput> HandleAsync<TOutput>(Func<TResource, Task<TOutput>> func);
    //    Task HandleAsync(Func<TResource, Task> func);
    //    void Handle(Action<TResource, RoutineClosure<TUserContext>> action);
    //    TOutput Handle<TOutput>(Func<TResource, RoutineClosure<TUserContext>, TOutput> func);
    //    Task<TOutput> HandleAsync<TOutput>(Func<TResource, RoutineClosure<TUserContext>, Task<TOutput>> func);
    //    Task HandleAsync(Func<TResource, RoutineClosure<TUserContext>, Task> func);
    //}

    public interface ISqlConnectionHandler<TUserContext, TResource>
    {
        void Handle(Action<RoutineClosure<TUserContext>, TResource, Action> action);
        void Handle(Action<RoutineClosure<TUserContext>, TResource> action);
        void Handle(Action<TResource, Action> action);
        void Handle(Action<TResource> action);
        TOutput Handle<TOutput>(Func<RoutineClosure<TUserContext>, TResource, Action, TOutput> func);
        TOutput Handle<TOutput>(Func<RoutineClosure<TUserContext>, TResource, TOutput> func);
        TOutput Handle<TOutput>(Func<TResource, Action, TOutput> func);
        TOutput Handle<TOutput>(Func<TResource, TOutput> func);
        Task HandleAsync(Action<RoutineClosure<TUserContext>, TResource, Action> action);
        Task HandleAsync(Action<RoutineClosure<TUserContext>, TResource> action);
        Task HandleAsync(Action<TResource, Action> action);
        Task HandleAsync(Action<TResource> action);
        Task<TOutput> HandleAsync<TOutput>(Func<RoutineClosure<TUserContext>, TResource, Action, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<RoutineClosure<TUserContext>, TResource, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TResource, Action, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TResource, TOutput> func);
    }
}