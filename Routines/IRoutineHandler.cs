namespace DashboardCode.Routines
{
    public interface IRoutineHandler<TClosure, TUserContext> : IHandler<TClosure>, IHandler<TClosure, RoutineClosure<TUserContext>>
    {
    }

    public interface IRoutineHandlerAsync<TClosure, TUserContext> : IHandlerAsync<TClosure>, IHandlerAsync<TClosure, RoutineClosure<TUserContext>>
    {
    }

    public interface IRoutineHandlerOmni<TClosure, TUserContext> : IHandlerOmni<TClosure>, IHandlerOmni<TClosure, RoutineClosure<TUserContext>>
    {
    }
}