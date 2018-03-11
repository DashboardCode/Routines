using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public interface IRoutineHandler<TClosure>
    {
        void Handle(Action<TClosure> action);
        TOutput Handle<TOutput>(Func<TClosure, TOutput> func);
        Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func);
    }

    public class RoutineHandlerSilent<TClosure> : IRoutineHandler<TClosure>
    {
        private readonly TClosure closure;
        private readonly ExceptionHandler exceptionHandler;
        private readonly Func<(Action, Action)> start;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="closure"></param>
        /// <param name="exceptionHandler"></param>
        /// <param name="start">returns logOnSuccess and onFailure (Used for: a. finish activity record b. trigger buffer flash)</param>
        public RoutineHandlerSilent(
            TClosure closure,
            ExceptionHandler exceptionHandler,
            Func<(Action, Action)> start)
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
                            onSuccess();
                        },
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
                            onSuccess();
                        },
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
                                    onSuccess()
                                );
                        },
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

    public class RoutineHandlerVerbose<TClosure> : IRoutineHandler<TClosure>
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
        public RoutineHandlerVerbose(
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
                        },
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
                        },
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
                        },
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