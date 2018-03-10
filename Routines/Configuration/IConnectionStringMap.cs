namespace DashboardCode.Routines.Configuration
{
    /// <summary>
    /// Special case (it can be done without it using common configuration access methods, but there is a tradition to dermine a specific method to access connection strings)
    /// </summary>
    public interface IConnectionStringMap
    {
        string GetConnectionString(string name);
    }
}