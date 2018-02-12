using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Injected
{
    public class RoutineHandler<TClosure>
    {
        private readonly IExceptionHandler exceptionHandler;
        private readonly object input;
        private readonly TClosure closure;
        private readonly IRoutineLogging routineLogging;

        public RoutineHandler(
            IExceptionHandler exceptionHandler,
            IRoutineLogging routineLogging,
            TClosure closure,
            object input)
        {
            this.input = input;
            this.closure = closure;
            this.exceptionHandler = exceptionHandler;
            this.routineLogging = routineLogging;
        }
        public void Handle(Action<TClosure> action)
        {
            var (logOnSuccess, logOnFailure) = routineLogging.LogStart(input);
            exceptionHandler.Handle(
                () =>
                {
                    action(closure);
                    logOnSuccess(null);
                },
                logOnFailure
            );
        }
        public TOutput Handle<TOutput>(Func<TClosure, TOutput> func)
        {
            var @value = default(TOutput);
            var (logOnSuccess, logOnFailure) = routineLogging.LogStart(input);
            exceptionHandler.Handle(
                () =>
                {
                    @value = func(closure);
                    logOnSuccess(@value);
                },
                logOnFailure
            );
            return @value;
        }
        public async Task<TOutput> HandleAsync<TOutput>(Func<TClosure, TOutput> func)
        {
            var @value = Task.Run(() =>
            {
                var output = default(TOutput);
                var (logOnSuccess, logOnFailure) = routineLogging.LogStart(input);
                exceptionHandler.Handle(
                    () =>
                    {
                        output = func(closure);
                        logOnSuccess(output);
                    },
                    logOnFailure
                );
                return output;
            });
            return await @value;
        }
    }
}