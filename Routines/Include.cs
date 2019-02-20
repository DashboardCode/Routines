namespace DashboardCode.Routines
{
    public delegate void Include<T>(Chain<T> chain);

    public delegate void Include<T,TP>(Chain<T> chain, TP tp);
}