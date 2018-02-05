namespace DashboardCode.Routines
{
    public interface IGFactory<TParam>
    {
        TResult Create<TResult>(TParam param);
    }
}
