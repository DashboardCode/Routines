namespace DashboardCode.Routines
{
    public interface IRoutineHandler<TClosure, TUserContext> : IHandler<TClosure>, IHandler<TClosure, RoutineClosure<TUserContext>>
    {
    }
}