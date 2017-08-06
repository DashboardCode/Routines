namespace DashboardCode.Routines
{
    public interface IContainer
    {
        T Resolve<T>() where T : new();
    }
}