namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationContainerFactory
    {
        ConfigurationContainer Create(MemberTag memberTag, string @for);
    }
}