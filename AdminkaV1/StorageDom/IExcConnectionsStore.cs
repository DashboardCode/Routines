namespace AdminkaV1.StorageDom
{
    public interface IExcConnectionsStore
    {
        Task DeleteAsync(string key);
        Task<ExcConnection?> GetAsync(string key);
        Task SetAsync(ExcConnection excConnection);
    }
}
