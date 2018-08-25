namespace DashboardCode.Routines
{
    public interface IGFactory<TInput>
    {
        TOutput Create<TOutput>(TInput input);
    }

    public interface IGWithConstructorFactory<TInput>
    {
        TOutput Create<TOutput>(TInput input) where TOutput : new();
    }
}