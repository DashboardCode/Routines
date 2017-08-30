namespace DashboardCode.Routines
{
    /// <summary>
    /// Now IProgress used instead of this.
    /// </summary>
    public interface IBuilder<in T> 
    {
        void Build(T t);
    }
}