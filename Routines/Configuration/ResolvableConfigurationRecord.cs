namespace DashboardCode.Routines.Configuration
{
    // TODO: replace with ResolvableRecord
    public interface IResolvableConfigurationRecord
    {
        string Namespace { get; }
        string Type      { get; }
        string Value     { get; }
    }

    //public struct ResolvableRecord
    //{
    //    public string Namespace;
    //    public string Type;
    //    public string Value;
    //}

    //public interface IResolvableRecord
    //{
    //    ResolvableRecord GetResolvableRecord();
    //}
}
