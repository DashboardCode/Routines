using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class StorageMetaService : IStorageMetaService
    {
        readonly Dictionary<string, IOrmEntitySchemaAdapter> entityMetas;
        readonly IMutableModel mutableModel;
        Func<Exception, IOrmEntitySchemaAdapter, string, StorageResult> analyze;

        private class RelationDbSchemaAdapter : IOrmEntitySchemaAdapter
        {
            string[] Binaries;
            string[] Keys;
            string[] Requireds;
            string SchemaName;
            string TableName;
            Dictionary<string, (string[], string)> Constraints;
            Dictionary<string, string[]> Uniques;
            public RelationDbSchemaAdapter(IEntityType entityType)
            {
                SchemaName = entityType.Relational().Schema;
                TableName = entityType.Relational().TableName;
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
                    foreach(var c in constraints)
                         Constraints.Add(c.Name, (c.Fields, c.Message));
                // ----------------------------------------------------------------------------------------------------------
                Uniques = new Dictionary<string, string[]>();
                foreach (var index in entityType.GetIndexes())
                    if (index.IsUnique)
                        Uniques.Add(index.SqlServer().Name, index.Properties.Select(e => e.Name).ToArray());
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
                return default(string[]);
            }

            public (string[] Attributes, string Message) GetConstraint(string name)
            {
                if (Constraints.TryGetValue(name, out (string[],string) properties))
                    return properties;
                return default((string[], string));
            }
        }

        public StorageMetaService(Func<Exception, IOrmEntitySchemaAdapter, string, StorageResult> analyze)
        {
            this.analyze = analyze;
            this.entityMetas = new Dictionary<string, IOrmEntitySchemaAdapter>();

            //TODO:  constraints and unique indexes list should be integrated with configuration files or get from db directly

            //var serviceCollection = new ServiceCollection();
            //var serviceProvider = serviceCollection.BuildServiceProvider();
            //var conventionSet2 = serviceProvider.GetService<ConventionSet>();

            var conventionSet = new ConventionSet();
            var modelBuilder = new ModelBuilder(conventionSet);
            AdminkaDbContext.BuildModel(modelBuilder);
            mutableModel = modelBuilder.Model;

            var entityTypes = mutableModel.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var lastIndexOfPoint = entityType.Name.LastIndexOf('.');
                var storageModel = new RelationDbSchemaAdapter(entityType);
                entityMetas.Add(entityType.Name, storageModel);
            }
        }

        public StorageResult Analyze<TEntity>(Exception ex)
        {
            var entityMeta =  entityMetas[typeof(TEntity).FullName];
            return analyze(ex, entityMeta, "");
        }

        public IOrmEntitySchemaAdapter<TEntity> GetOrmMetaAdapter<TEntity>() where TEntity : class
        {
            var entityMeta = entityMetas[typeof(TEntity).FullName];
            var @output = new OrmMetaAdapter<TEntity>(mutableModel, entityMeta);
            return @output;
        }
    }
}