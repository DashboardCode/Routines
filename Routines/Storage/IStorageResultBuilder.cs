using System;

namespace DashboardCode.Routines.Storage
{
    public interface IStorageResultBuilder
    {
        StorageResult Build();
        void AddTruncationError(string entityName);
        void AddConcurrencyError(string entityName);
        void AddPkDuplicateError(string constraint, string table);
        void AddPkDuplicateError(string entityName);
        void AddNullError(string column, string table);
        void AddUniqueIndexViolations(string index, string table);
        void AddUniqueConstraintViolations(string constraint, string table, string duplicateValue);
        void AddCheckConstraintViolations(string constraint, string table);
        void AddNullPrimaryOrAlternateKey(string entity);
        void AddNullPrimaryOrAlternateKey(string entity, string field);
        void AddValidation(string entity, string field, string message);
    }
}