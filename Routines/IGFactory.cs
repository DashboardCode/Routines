namespace DashboardCode.Routines
{
    public interface IGFactory<T>
    {
        TOutput Create<TOutput>(T input);
    }
}