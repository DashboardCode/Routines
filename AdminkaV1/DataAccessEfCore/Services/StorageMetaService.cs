using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class StorageMetaService
    {
        readonly List<StorageModel> storageModels;
        Func<Exception, StorageModel, List<FieldMessage>> analyze;

        public StorageMetaService(Func<Exception, StorageModel, List<FieldMessage>> analyze)
        {
            this.analyze = analyze;

            //TODO:  constraints and unique indexes list should be integrated with configuration files or get from db directly

            //var serviceCollection = new ServiceCollection();
            //var serviceProvider = serviceCollection.BuildServiceProvider();
            //var conventionSet2 = serviceProvider.GetService<ConventionSet>();

            var conventionSet = new ConventionSet();
            var modelBuilder = new ModelBuilder(conventionSet);
            AdminkaDbContext.BuildModel(modelBuilder);
            IMutableModel mutableModel = modelBuilder.Model;

            this.storageModels = new List<StorageModel>();
            var entityTypes = mutableModel.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var lastIndexOfPoint = entityType.Name.LastIndexOf('.');
                var storageModel = new StorageModel()
                {
                    Entity = new Entity()
                    {
                        Name = entityType.Name.Substring(lastIndexOfPoint + 1),
                        Namespace = entityType.Name.Substring(0, lastIndexOfPoint - 1)
                    }
                };
                storageModels.Add(storageModel);
                //storageModels.FirstOrDefault(e => e.Entity.Namespace + "." + e.Entity.Name == entityType.Name);
                if (storageModel != null)
                {
                    storageModel.SchemaName = entityType.Relational().Schema;
                    storageModel.TableName = entityType.Relational().TableName;
                    // ----------------------------------------------------------------------------------------------------------
                    var constraintsAnnotation = entityType.FindAnnotation("Constraints");
                    if (constraintsAnnotation != null && constraintsAnnotation.Value is Constraint[] constraints)
                        storageModel.Constraints = constraints;
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
                        //if (property)
                        //    binaries.Add(property.Name);
                    };
                    if (requireds.Count > 0)
                        storageModel.Requireds = requireds.ToArray();
                    if (keys.Count > 0)
                        storageModel.Keys = keys.ToArray();
                    if (binaries.Count > 0)
                        storageModel.Keys = keys.ToArray();
                    // ----------------------------------------------------------------------------------------------------------
                    var uniques = new List<Unique>();
                    foreach (var index in entityType.GetIndexes())
                        if (index.IsUnique)
                            uniques.Add(new Unique { IndexName = index.SqlServer().Name, Fields = index.Properties.Select(e => e.Name).ToArray() });
                    if (uniques.Count > 0)
                        storageModel.Uniques = uniques.ToArray();
                }
            }
        }

        public StorageModel FindStorageModel<TEntity>()
        {
            var storageModel = storageModels.FirstOrDefault(e => e.Entity.Namespace + "." + e.Entity.Name == typeof(TEntity).FullName);
            return storageModel;
        }

        public List<FieldMessage> Analyze<TEntity>(Exception ex)
        {
            var storageModel = FindStorageModel<TEntity>();
            return (storageModel == null) ? new List<FieldMessage>() : analyze(ex, storageModel);
        }
    }
}
