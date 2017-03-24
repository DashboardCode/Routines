namespace Vse.Routines.Configuration
{
    public interface IResolvable
    {
        string Namespace { get; }
        string Type { get; }
        string Value { get; }
    }
}
