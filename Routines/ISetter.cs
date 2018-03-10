namespace DashboardCode.Routines
{
    public interface ISetter<in T> 
    {
        void Set(T t);
    }
}