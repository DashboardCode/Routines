using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public class HandlerVerbose<TClosure> : IHandler<TClosure>
    {
        private readonly TClosure closure;
        private readonly ExceptionHandler exceptionHandler;
        private readonly Func<(Action<object>, Action)> start;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="closure"></param>
        /// <param name="exceptionHandler"></param>
        /// <param name="start">returns logOnSuccess and onFailure (Used for: a. finish activity record b. trigger buffer flash)</param>
        public HandlerVerbose(
            TClosure closure,
            ExceptionHandler exceptionHandler,
            Func<(Action<object>, Action)> start)
        {
            this.closure = closure;
            this.exceptionHandler = exceptionHandler;
            this.start = start;
        }

        public void Handle(Action<TClosure> action)
        {
            exceptionHandler.Handle(
                () =>
                {
                    var (onSuccess, onFailure) = start();
                    return (
                        () => {
                            action(closure);
                            onSuccess(null);
                        }
                    ,
                        isSuccess =>
                        {
                            if (!isSuccess)
                                onFailure();
                        }
                    );
                }
            );
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var @value = default(TOutput);
            exceptionHandler.Handle(
                () =>
                {
                    var (onSuccess, onFailure) = start();
                    return (
                        () => {
                            @value = func(closure);
                            onSuccess(@value);
                        }
                    ,
                        isSuccess =>
                        {
                            if (!isSuccess)
                                onFailure();
                        }
                    );
                }
            );
            return @value;
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var successTask = default(Task<TOutput>);
            exceptionHandler.Handle(
                () =>
                {
                    var (onSuccess, onFailure) = start();
                    return (
                        () => {
                            successTask = func(closure);
                            successTask.ContinueWith(
                                t =>
                                    onSuccess(t.Result)
                                );
                        }
                    ,
                        isSuccess =>
                        {
                            if (!isSuccess)
                                onFailure();
                        }
                    );
                }
            );
            return successTask;
        }

        public Task HandleAsync(Func<TClosure, Task> func)
        {
            var successTask = default(Task);
            exceptionHandler.Handle(
                () =>
                {
                    var (onSuccess, onFailure) = start();
                    return (
                        () => {
                            successTask = func(closure);
                            successTask.ContinueWith(
                                t =>
                                    onSuccess(null)
                                );
                        }
                    ,
                        isSuccess =>
                        {
                            if (!isSuccess)
                                onFailure();
                        }
                    );
                }
            );
            return successTask;
        }
    }
}
