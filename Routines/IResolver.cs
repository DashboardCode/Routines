namespace DashboardCode.Routines
{
    public interface IResolver
    {
        T Resolve<T>() where T : new();
    }
}