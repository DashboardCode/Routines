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
    public class EntityMetaServiceContainer : IEntityMetaServiceContainer
    {
        readonly Dictionary<string, OrmEntitySchemaAdapter> relationDbSchemaAdapters;
        readonly IMutableModel mutableModel;
        readonly Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze;

        public EntityMetaServiceContainer(Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze)
        {
            this.analyze = analyze;
            this.relationDbSchemaAdapters = new Dictionary<string, OrmEntitySchemaAdapter>();

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
                var relationDbSchemaAdapter = new OrmEntitySchemaAdapter(entityType);
                relationDbSchemaAdapters.Add(entityType.Name, relationDbSchemaAdapter);
            }
        }

        public IEntityMetaService<TEntity> Resolve<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var ormEntitySchemaAdapter = relationDbSchemaAdapters[type.FullName];
            var ormMetaAdapter = new OrmMetaAdapter<TEntity>(mutableModel, ormEntitySchemaAdapter);
            var entityStorageMetaService = new EntityStorageMetaService<TEntity>(ormMetaAdapter, type, mutableModel, analyze);
            return entityStorageMetaService;
        }

        class EntityStorageMetaService<TEntity> : IEntityMetaService<TEntity> where TEntity : class
        {
            readonly OrmMetaAdapter<TEntity> ormMetaAdapter;
            readonly Type type;
            readonly IMutableModel mutableModel;
            readonly Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze;
            public EntityStorageMetaService(
                OrmMetaAdapter<TEntity> ormMetaAdapter,
                Type type,
                IMutableModel mutableModel,
                Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze)
            {
                this.ormMetaAdapter = ormMetaAdapter;
                this.type = type;
                this.analyze = analyze;
                this.mutableModel = mutableModel;
            }

            public StorageResult Analyze(Exception ex)
            {
                var storageResult = analyze(ex, type, ormMetaAdapter, "");
                return storageResult;
            }

            public IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter()
            {
                var @output = new OrmMetaAdapter<TEntity>(mutableModel, ormMetaAdapter);
                return @output;
            }
        }

        class OrmEntitySchemaAdapter : IOrmEntitySchemaAdapter
        {
            string[] Binaries;
            string[] Keys;
            string[] Requireds;
            string SchemaName;
            string TableName;
            Dictionary<string, (string[], string)> Constraints;
            Dictionary<string, string[]> Uniques;
            public OrmEntitySchemaAdapter(IEntityType entityType)
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
                    foreach (var c in constraints)
                        Constraints.Add(c.Name, (c.Fields, c.Message));
                // ----------------------------------------------------------------------------------------------------------
                Uniques = new Dictionary<string, string[]>();
                var indexes = entityType.GetIndexes();
                foreach (var index in indexes)
                    if (index.IsUnique)
                    {
                        var sqlServerAnnotations = index.SqlServer();
                        var indexName = sqlServerAnnotations.Name;
                        var fields = index.Properties.Select(e => e.Name).ToArray();
                        Uniques.Add(indexName, fields);
                    }
                // table.UniqueConstraint("AK_ParentRecords_FieldCA", x => x.FieldCA);
                // table.UniqueConstraint("AK_ParentRecords_FieldCB1_FieldCB2", x => new { x.FieldCB1, x.FieldCB2 });
                var annotations = entityType.GetAnnotations();
                var entitySqlServerAnnotations = entityType.SqlServer();
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
                    var entityKeySqlServer = entityKey.SqlServer();
                    var uniqueConstraintName = entityKeySqlServer.Name;
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
                return default(string[]);
            }

            public (string[] Attributes, string Message) GetConstraint(string name)
            {
                if (Constraints.TryGetValue(name, out (string[], string) properties))
                    return properties;
                return default((string[], string));
            }
        }
    }
}