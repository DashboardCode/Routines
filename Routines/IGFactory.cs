namespace DashboardCode.Routines
{
    public interface IGFactory<TInput>
    {
        TOutput Create<TOutput>(TInput input);
    }
}