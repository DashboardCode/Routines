﻿using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class HandlerVerbose<TClosure> : IHandlerOmni<TClosure>
    {
        private readonly TClosure closure;
        private readonly IExceptionHandler exceptionHandler;
        private readonly Func<(Action<object> onSuccess, Action onFailure)> start;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="closure"></param>
        /// <param name="exceptionHandler"></param>
        /// <param name="start">returns logOnSuccess and onFailure (Used for: a. finish activity record b. trigger buffer flash)</param>
        public HandlerVerbose(
            TClosure closure,
            IExceptionHandler exceptionHandler,
            Func<(Action<object> onSuccess, Action onFailure)> start)
        {
            this.closure = closure;
            this.exceptionHandler = exceptionHandler;
            this.start = start;
        }

        public void Handle(Action<TClosure> action)
        {
            var (onSuccess, onFailure) = start();
            exceptionHandler.Handle(
                        () => {
                            action(closure);
                            onSuccess(null);
                        },
                        isSuccess => {
                            if (!isSuccess)
                                onFailure();
                        }
            );
        }

        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var @value = default(TOutput);
            var (onSuccess, onFailure) = start();
            exceptionHandler.Handle(
                        () => {
                            @value = func(closure);
                            onSuccess(@value);
                        },
                        isSuccess => {
                            if (!isSuccess)
                                onFailure();
                        }
            );
            return @value;
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, Task<TOutput>> func)
        {
            var @value = default(TOutput);
            var (onSuccess, onFailure) = start();
            await exceptionHandler.HandleAsync(
                        async () => {
                            @value = await func(closure);
                            onSuccess(@value);
                        }, 
                        isSuccess => {
                            if (!isSuccess) { 
                                onFailure();
                            }
                        }
            );
            return @value;
        }

        public async Task HandleAsync(Func<TClosure, Task> func)
        {
            var (onSuccess, onFailure) = start();
            await exceptionHandler.HandleAsync(
                        async () => {
                            await func(closure);
                            onSuccess(null);
                        }, 
                        isSuccess => {
                            if (!isSuccess)
                                onFailure();
                        }
            );
        }
    }
}