using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public interface IErrorBuilder
    {
        void AddTruncationError();
        void AddConcurrencyException();
        void AddPkDuplicateError(string constraint, string table);
        void AddPkDuplicateError();
        void AddNullError(string column, string table);
        void AddUniqueIndexViolations(string index, string table);
        void AddUniqueConstraintViolations(string constraint, string table);
        void AddCheckConstraintViolations(string constraint, string table);
        void AddNullPrimaryOrAlternateKey(string entity);
        void AddNullPrimaryOrAlternateKey(string entity, string field);
    }

    public class ErrorBuilder : IErrorBuilder
    {
        List<FieldMessage> fieldsErrors;
        StorageModel storageModel;
        string genericErrorField;
        public ErrorBuilder(List<FieldMessage> fieldsErrors, StorageModel storageModel, string genericErrorField)
        {
            this.fieldsErrors = fieldsErrors;
            this.storageModel = storageModel;
            this.genericErrorField = genericErrorField;
        }

        public void AddNullPrimaryOrAlternateKey(string entity)
        {
            if (storageModel.TableName == entity)
                fieldsErrors.Add(genericErrorField, "ID or alternate id has no value");
        }

        public void AddNullPrimaryOrAlternateKey(string entity, string field)
        {
            if (storageModel.TableName == entity)
                fieldsErrors.Add(field, "ID or alternate id has no value");
        }

        public void AddConcurrencyException()
        {
            fieldsErrors.Add(genericErrorField, "The record you are attempted to edit is currently being modified by another user. The save operation was canceled! Refresh page and reload form before continue.");
        }

        public void AddTruncationError()
        {
            fieldsErrors.Add("", "Some of text " + (storageModel.Binaries != null ? "" : "(or binaries) ") + "fields cannot fit into DB");
        }

        public void AddPkDuplicateError(string constraint, string table)
        {
            if (table.Contains(storageModel.SchemaName + "." + storageModel.TableName) && storageModel.Keys != null)
            {
                if (storageModel.Keys.Length == 1)
                {
                    fieldsErrors.Add(storageModel.Keys[0], $"Allready exists in DB");
                }
                else
                {
                    var csv = string.Join(",", storageModel.Keys);
                    foreach (var f in storageModel.Keys)
                        fieldsErrors.Add(f, "Allready exists is DB (multicolumn key: " + csv + ")");
                }
            }
        }

        public void AddPkDuplicateError()
        {
            if (storageModel.Keys != null)
                if (storageModel.Keys.Length == 1)
                {
                    fieldsErrors.Add(storageModel.Keys[0], $"ID exists in database");
                }
                else
                {
                    var csv = string.Join(",", storageModel.Keys);
                    foreach (var a in storageModel.Keys)
                        fieldsErrors.Add(a, "ID exists in database (multiple fields: " + csv + ")");
                }
        }

        public void AddNullError(string column, string table)
        {
            if (table.Contains(storageModel.SchemaName + "." + storageModel.TableName))
            {
                if (storageModel.Requireds != null && storageModel.Requireds.Contains(column))
                {
                    fieldsErrors.Add(column, $"Is required!");
                }
            }
        }

        public void AddUniqueIndexViolations(string index, string table)
        {
            if (table.Contains(storageModel.SchemaName + "." + storageModel.TableName) && storageModel.Uniques != null)
            {
                var unique = storageModel.Uniques.FirstOrDefault(e => e.IndexName == index);
                if (unique != null)
                {
                    if (unique.Fields.Length == 1)
                    {
                        fieldsErrors.Add(unique.Fields[0], $"Allready used");
                    }
                    else
                    {
                        var csv = string.Join(",", unique.Fields);
                        foreach (var f in unique.Fields)
                            fieldsErrors.Add(f, "Allready used (multiple fields: " + csv + ")");
                    }
                }
            }
        }

        public void AddUniqueConstraintViolations(string constraint, string table)
        {
            if (table.Contains(storageModel.SchemaName + "." + storageModel.TableName) && storageModel.Uniques != null)
            {
                var unique = storageModel.Uniques.FirstOrDefault(e => e.IndexName == constraint);
                if (unique != null)
                {
                    if (unique.Fields.Length == 1)
                    {
                        fieldsErrors.Add(unique.Fields[0], $"Allready used");
                    }
                    else
                    {
                        var csv = string.Join(",", unique.Fields);
                        foreach (var f in unique.Fields)
                            fieldsErrors.Add(f, "Allready used (multiple fields: " + csv + ")");
                    }
                }
            }
        }

        public void AddCheckConstraintViolations(string constraint, string table)
        {
            if (storageModel.Constraints != null)
            {
                if (table == storageModel.SchemaName + "." + storageModel.TableName)
                {
                    var ck = storageModel.Constraints.FirstOrDefault(e => e.Name == constraint);
                    if (ck != null)
                    {
                        if (ck.Fields == null || ck.Fields.Length == 0)
                        {
                            fieldsErrors.Add(genericErrorField, ck.Message);
                        }
                        if (ck.Fields.Length == 1)
                        {
                            fieldsErrors.Add(ck.Fields[0], ck.Message);
                        }
                        else
                        {
                            var csv = string.Join(",", ck.Fields);
                            foreach (var f in ck.Fields)
                                fieldsErrors.Add(f, ck.Message);
                        }
                    }
                }
            }
        }
    }
}