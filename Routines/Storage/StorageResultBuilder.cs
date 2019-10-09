using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public class StorageResultBuilder : IStorageResultBuilder
    {
        List<FieldMessage> fieldMessages;
        IOrmEntitySchemaAdapter relationalEntitySchemaAdapter;
        readonly string genericErrorField;
        readonly Type entityType;
        readonly Exception exception;
        public StorageResultBuilder(Exception exception, Type entityType, IOrmEntitySchemaAdapter relationalEntitySchemaAdapter, string genericErrorField)
        {
            this.entityType = entityType;
            this.exception = exception;
            this.fieldMessages = new List<FieldMessage>(); ;
            this.relationalEntitySchemaAdapter = relationalEntitySchemaAdapter;
            this.genericErrorField = genericErrorField;
        }

        public virtual void AddNullPrimaryOrAlternateKey(string entityName)
        {
            if (this.entityType.Name == entityName)
                fieldMessages.Add(genericErrorField, "ID or alternate id has no value");
        }

        public virtual void AddNullPrimaryOrAlternateKey(string entityName, string field)
        {
            if (this.entityType.Name == entityName)
                fieldMessages.Add(field, "ID or alternate id has no value");
        }

        public virtual void AddConcurrencyError()
        {
            fieldMessages.Add(genericErrorField, "The record you are attempted to edit is currently being modified by another user. The save operation was canceled! Refresh page and reload form before continue.");
        }

        public virtual void AddTruncationError()
        {
            fieldMessages.Add("", "Some of text " + (relationalEntitySchemaAdapter.GetBinaries() != null ? "" : "(or binaries) ") + "fields cannot fit into DB");
        }

        public virtual void AddPkDuplicateError(string constraint, string table)
        {
            var tableSchemaName = relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName;
            if (table.Contains(tableSchemaName) && relationalEntitySchemaAdapter.GetKeys() != null)
            {
                if (relationalEntitySchemaAdapter.GetKeys().Length == 1)
                {
                    fieldMessages.Add(relationalEntitySchemaAdapter.GetKeys()[0], $"Allready exists in DB");
                }
                else
                {
                    var csv = string.Join(",", relationalEntitySchemaAdapter.GetKeys());
                    foreach (var f in relationalEntitySchemaAdapter.GetKeys())
                        fieldMessages.Add(f, "Allready exists is DB (multicolumn key: " + csv + ")");
                }
            }
        }

        public virtual void AddPkDuplicateError()
        {
            if (relationalEntitySchemaAdapter.GetKeys() != null)
                if (relationalEntitySchemaAdapter.GetKeys().Length == 1)
                {
                    fieldMessages.Add(relationalEntitySchemaAdapter.GetKeys()[0], $"ID exists in database");
                }
                else
                {
                    var csv = string.Join(",", relationalEntitySchemaAdapter.GetKeys());
                    foreach (var a in relationalEntitySchemaAdapter.GetKeys())
                        fieldMessages.Add(a, "ID exists in database (multiple fields: " + csv + ")");
                }
        }

        public virtual void AddNullError(string column, string table)
        {
            var tableSchemaName = relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName;
            if (table.Contains(tableSchemaName))
            {
                if (relationalEntitySchemaAdapter.GetRequireds() != null && relationalEntitySchemaAdapter.GetRequireds().Contains(column))
                {
                    fieldMessages.Add(column, $"Is required!");
                }
            }
        }

        public virtual void AddUniqueIndexViolations(string index, string table)
        {
            var tableSchemaName = relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName;
            if (table.Contains(tableSchemaName))
            {
                var properties = relationalEntitySchemaAdapter.GetUnique(index);
                if (properties != null)
                {
                    if (properties.Length == 1)
                    {
                        fieldMessages.Add(properties[0], $"Allready used");
                    }
                    else
                    {
                        var csv = string.Join(",", properties);
                        foreach (var p in properties)
                            fieldMessages.Add(p, "Allready used (multiple fields: " + csv + ")");
                    }
                }
            }
        }

        public virtual void AddUniqueConstraintViolations(string constraint, string table)
        {
            var tableSchemaName = relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName;
            if (table.Contains(tableSchemaName))
            {
                var properties = relationalEntitySchemaAdapter.GetUnique(constraint);
                if (properties != null)
                {
                    if (properties.Length == 1)
                        fieldMessages.Add(properties[0], $"Allready used");
                    else
                    {
                        var csv = string.Join(",", properties);
                        foreach (var p in properties)
                            fieldMessages.Add(p, "Allready used (multiple fields: " + csv + ")");
                    }
                }
            }
        }

        public virtual void AddCheckConstraintViolations(string constraintName, string table)
        {
            var tableSchemaName = relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName;
            if (table == tableSchemaName)
            {
                var (Attributes, Message) = relationalEntitySchemaAdapter.GetConstraint(constraintName);
                if (Attributes != null)
                {
                    if (Attributes.Length == 1)
                        fieldMessages.Add(Attributes[0], Message);
                    else
                    {
                        var csv = string.Join(",", Attributes);
                        foreach (var f in Attributes)
                            fieldMessages.Add(f, Message);
                    }
                }
            }
        }

        public static StorageResult AnalyzeExceptionRecursive(
            Exception exception, Type entityType, IOrmEntitySchemaAdapter ormEntitySchemaAdapter, 
            string genericErrorField, Action<Exception, IStorageResultBuilder> parser = null)
        {
            return StorageResultExtensions.AnalyzeExceptionRecursive(
                exception,
                (recursiveParse) =>
                {
                    
                    var storageResultBuilder = new StorageResultBuilder(exception, entityType, ormEntitySchemaAdapter, genericErrorField);
                    recursiveParse(storageResultBuilder);
                    return storageResultBuilder.Build();
                },
                parser
            );
        }

        public StorageResult Build()
        {
            return new StorageResult(exception, fieldMessages);
        }
    }
}