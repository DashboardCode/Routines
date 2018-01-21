using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public class StorageResultBuilder : IStorageResultBuilder
    {
        List<FieldMessage> fieldMessages;
        IOrmEntitySchemaAdapter relationalEntitySchemaAdapter;
        string genericErrorField;
        Exception exception;
        public StorageResultBuilder(Exception exception, IOrmEntitySchemaAdapter relationalEntitySchemaAdapter, string genericErrorField)
        {
            this.exception = exception;
            this.fieldMessages = new List<FieldMessage>(); ;
            this.relationalEntitySchemaAdapter = relationalEntitySchemaAdapter;
            this.genericErrorField = genericErrorField;
        }

        public virtual void AddNullPrimaryOrAlternateKey(string entity)
        {
            if (relationalEntitySchemaAdapter.GetTableName().TableName == entity)
                fieldMessages.Add(genericErrorField, "ID or alternate id has no value");
        }

        public virtual void AddNullPrimaryOrAlternateKey(string entity, string field)
        {
            if (relationalEntitySchemaAdapter.GetTableName().TableName == entity)
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
            if (table.Contains(relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName) && relationalEntitySchemaAdapter.GetKeys() != null)
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
            if (table.Contains(relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName))
            {
                if (relationalEntitySchemaAdapter.GetRequireds() != null && relationalEntitySchemaAdapter.GetRequireds().Contains(column))
                {
                    fieldMessages.Add(column, $"Is required!");
                }
            }
        }

        public virtual void AddUniqueIndexViolations(string index, string table)
        {
            if (table.Contains(relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName))
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
            if (table.Contains(relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName))
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
            if (table == relationalEntitySchemaAdapter.GetTableName().SchemaName + "." + relationalEntitySchemaAdapter.GetTableName().TableName)
            {
                var constraint = relationalEntitySchemaAdapter.GetConstraint(constraintName);
                if (constraint.Attributes != null)
                {
                    if (constraint.Attributes.Length == 1)
                        fieldMessages.Add(constraint.Attributes[0], constraint.Message);
                    else
                    {
                        var csv = string.Join(",", constraint.Attributes);
                        foreach (var f in constraint.Attributes)
                            fieldMessages.Add(f, constraint.Message);
                    }
                }
            }
        }

        public static StorageResult AnalyzeExceptionRecursive(Exception exception, IOrmEntitySchemaAdapter relationalEntitySchemaAdapter, string genericErrorField, Action<Exception, IStorageResultBuilder> parser = null)
        {
            return StorageResultExtensions.AnalyzeExceptionRecursive(
                exception,
                (recursiveParse) =>
                {
                    
                    var storageResultBuilder = new StorageResultBuilder(exception, relationalEntitySchemaAdapter, genericErrorField);
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