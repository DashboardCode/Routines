namespace Vse.Routines
{
    public delegate void Include<T>(Includable<T> includable) where T : class;
}
