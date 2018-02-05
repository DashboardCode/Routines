namespace DashboardCode.Routines.Configuration
{
    public interface IConfigurationFactory
    {
        ConfigurationContainer ComposeSpecify(MemberTag memberTag, string @for);
    }
}
