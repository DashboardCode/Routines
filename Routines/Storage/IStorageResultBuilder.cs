namespace DashboardCode.Routines.Storage
{
    public interface IStorageResultBuilder
    {
        StorageResult Build();
        void AddTruncationError();
        void AddConcurrencyError();
        void AddPkDuplicateError(string constraint, string table);
        void AddPkDuplicateError();
        void AddNullError(string column, string table);
        void AddUniqueIndexViolations(string index, string table);
        void AddUniqueConstraintViolations(string constraint, string table);
        void AddCheckConstraintViolations(string constraint, string table);
        void AddNullPrimaryOrAlternateKey(string entity);
        void AddNullPrimaryOrAlternateKey(string entity, string field);
    }
}