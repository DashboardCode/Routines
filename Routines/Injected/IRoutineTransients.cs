namespace Vse.Routines.Injected
{
    public interface IRoutineTransients
    {
        IRoutineLogging ResolveRoutineLogging();
        IExceptionHandler ResolveExceptionHandler();
    }

    public interface IRoutineTransients<out TStateService> : IRoutineTransients
    {
        TStateService ResolveStateService();
    }
}
