using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Logging
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

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var @value = default(TOutput);
            await exceptionHandler.HandleAsync(
                () =>
                {
                    var (onSuccess, onFailure) = start();
                    return (async () => {
                        @value = await func(closure);
                        onSuccess(@value);
                    }
                    , isSuccess =>
                    {
                        if (!isSuccess)
                            onFailure();
                    }
                    );
                }
            );
            return @value;
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            await exceptionHandler.HandleAsync(
                () =>
                {
                    var (onSuccess, onFailure) = start();
                    return (async () => {
                        await func(closure);
                        onSuccess(null);
                    }
                    , isSuccess =>
                    {
                        if (!isSuccess)
                            onFailure();
                    }
                    );
                }
            );
        }
    }
}
