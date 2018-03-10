namespace DashboardCode.Routines.Configuration
{
    /// <summary>
    /// Design compromise. I'll be mo happy to have there pure struct/ref types then interfaces, but this way configuration can be easy integrated with .NET Frameworsk System.Configuration API.
    /// </summary>
    public interface IResolvableConfigurationRecord
    {
        string Namespace { get; }
        string Type      { get; }
        string Value     { get; }
    }
}