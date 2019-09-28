using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DashboardCode.Routines.Storage.EfCore.Relational
{
    public class EntityMetaServiceContainer : IEntityMetaServiceContainer
    {
        readonly Dictionary<string, IOrmEntitySchemaAdapter> relationDbSchemaAdapters;
        readonly IMutableModel mutableModel;
        readonly Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze;
        

        public EntityMetaServiceContainer(
            Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze,
            Func<IMutableEntityType, IOrmEntitySchemaAdapter> ormEntitySchemaAdapterFactory,
            Action<ModelBuilder> buildModel 
            )
        {
            this.analyze = analyze;
            this.relationDbSchemaAdapters = new Dictionary<string, IOrmEntitySchemaAdapter>();

            //TODO:  constraints and unique indexes list should be integrated with configuration files or get from db directly

            var conventionSet = new ConventionSet();
            var modelBuilder = new ModelBuilder(conventionSet);
            buildModel(modelBuilder);
            mutableModel = modelBuilder.Model;

            var entityTypes = mutableModel.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var lastIndexOfPoint = entityType.Name.LastIndexOf('.');
                var relationDbSchemaAdapter = ormEntitySchemaAdapterFactory(entityType); //new OrmEntitySchemaAdapter(entityType);
                relationDbSchemaAdapters.Add(entityType.Name, relationDbSchemaAdapter);
            }
        }

        public IEntityMetaService<TEntity> Resolve<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var ormEntitySchemaAdapter = relationDbSchemaAdapters[type.FullName];

            var ormEntitySchemaAdapter2 = new OrmEntitySchemaAdapter<TEntity>(mutableModel, ormEntitySchemaAdapter);
            var entityStorageMetaService = new EntityStorageMetaService<TEntity>(ormEntitySchemaAdapter2, type, mutableModel, analyze);
            return entityStorageMetaService;
        }

        class EntityStorageMetaService<TEntity> : IEntityMetaService<TEntity> where TEntity : class
        {
            readonly OrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter;
            readonly Type type;
            readonly IMutableModel mutableModel;
            readonly Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze;
            public EntityStorageMetaService(
                OrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter,
                Type type,
                IMutableModel mutableModel,
                Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze)
            {
                this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
                this.type = type;
                this.analyze = analyze;
                this.mutableModel = mutableModel;
            }

            public StorageResult Analyze(Exception ex)
            {
                var storageResult = analyze(ex, type, ormEntitySchemaAdapter, "");
                return storageResult;
            }

            public IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter()
            {
                var @output = new OrmEntitySchemaAdapter<TEntity>(mutableModel, ormEntitySchemaAdapter);
                return @output;
            }
        }
    }
}
