using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DashboardCode.Routines.Storage.EfCore.Relational.SqlServer
{
    public class SqlServerOrmEntitySchemaAdapter : IOrmEntitySchemaAdapter
    {
        readonly string[] Binaries;
        readonly string[] Keys;
        readonly string[] Requireds;
        readonly string SchemaName;
        readonly string TableName;
        readonly Dictionary<string, (string[], string)> Constraints;
        readonly Dictionary<string, string[]> Uniques;
        public SqlServerOrmEntitySchemaAdapter(IEntityType entityType)
        {
            SchemaName = entityType.GetTableName();
            TableName = entityType.GetSchema();
            // ----------------------------------------------------------------------------------------------------------
            var requireds = new List<string>();
            var keys = new List<string>();
            var binaries = new List<string>();
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsNullable)
                    requireds.Add(property.Name);
                if (property.IsKey())
                    keys.Add(property.Name);
                if (property.ClrType == typeof(byte[]))
                    binaries.Add(property.Name);
            };
            if (requireds.Count > 0)
                Requireds = requireds.ToArray();
            if (keys.Count > 0)
                Keys = keys.ToArray();
            if (binaries.Count > 0)
                Binaries = binaries.ToArray();
            // ----------------------------------------------------------------------------------------------------------
            Constraints = new Dictionary<string, (string[], string)>();
            var constraintsAnnotation = entityType.FindAnnotation(Constraint.AnnotationName);
            if (constraintsAnnotation != null && constraintsAnnotation.Value is Constraint[] constraints)
                foreach (var c in constraints)
                    Constraints.Add(c.Name, (c.Fields, c.Message));
            // ----------------------------------------------------------------------------------------------------------
            Uniques = new Dictionary<string, string[]>();
            var indexes = entityType.GetIndexes();
            foreach (var index in indexes)
                if (index.IsUnique)
                {
                    //var sqlServerAnnotations = index.SqlServer();
                    var indexName = index.GetName();
                    var fields = index.Properties.Select(e => e.Name).ToArray();
                    Uniques.Add(indexName, fields);
                }
            // table.UniqueConstraint("AK_ParentRecords_FieldCA", x => x.FieldCA);
            // table.UniqueConstraint("AK_ParentRecords_FieldCB1_FieldCB2", x => new { x.FieldCB1, x.FieldCB2 });
            var annotations = entityType.GetAnnotations();
            //var entitySqlServerAnnotations = entityType.SqlServer();
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsNullable)
                    requireds.Add(property.Name);
                if (property.IsKey())
                    keys.Add(property.Name);
                if (property.ClrType == typeof(byte[]))
                    binaries.Add(property.Name);
            };
            var entityKeys = entityType.GetKeys();
            foreach (var entityKey in entityKeys)
            {
                //var entityKeySqlServer = entityKey.SqlServer();
                var uniqueConstraintName = entityKey.GetName();
                var fields = entityKey.Properties.Select(e => e.Name).ToArray();
                Uniques.Add(uniqueConstraintName, fields);
            }
        }

        public string[] GetBinaries()
        {
            return Binaries;
        }

        public string[] GetKeys()
        {
            return Keys;
        }

        public string[] GetRequireds()
        {
            return Requireds;
        }

        public (string SchemaName, string TableName) GetTableName()
        {
            return (SchemaName, TableName);
        }

        public string[] GetUnique(string name)
        {
            if (Uniques.TryGetValue(name, out string[] properties))
                return properties;
            return default;
        }

        public (string[] Attributes, string Message) GetConstraint(string name)
        {
            if (Constraints.TryGetValue(name, out (string[], string) properties))
                return properties;
            return default((string[], string));
        }
    }
}
