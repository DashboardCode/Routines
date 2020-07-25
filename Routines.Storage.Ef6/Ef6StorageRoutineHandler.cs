using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class Ef6StorageRoutineHandler<TUserContext, TDbContext> : MetaStorageRoutineHandler<TUserContext, TDbContext>
        where TDbContext : DbContext
    {
        static readonly RepositoryContainer<TDbContext> repositoryContainer = new RepositoryContainer<TDbContext>();
        static readonly OrmContainer<TDbContext> ormContainer = new OrmContainer<TDbContext>();

        public Ef6StorageRoutineHandler(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            Func<TDbContext> createDbContext,
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            ) :
                base(entityMetaServiceContainer, createDbContext, createDbContextForStorage, repositoryContainer, ormContainer,
                     routineHandler)
        {
        }
    }

    public class Ef6StorageRoutineHandlerAsync<TUserContext, TDbContext> : MetaStorageRoutineHandlerAsync<TUserContext, TDbContext>
        where TDbContext : DbContext
    {
        static readonly RepositoryContainer<TDbContext> repositoryContainer = new RepositoryContainer<TDbContext>();
        static readonly OrmContainer<TDbContext> ormContainer = new OrmContainer<TDbContext>();

        public Ef6StorageRoutineHandlerAsync(
            IEntityMetaServiceContainer entityMetaServiceContainer,
            Func<TDbContext> createDbContext,
            Func<(TDbContext, IAuditVisitor)> createDbContextForStorage,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler
            ) :
                base(entityMetaServiceContainer, createDbContext, createDbContextForStorage, repositoryContainer, ormContainer,
                     routineHandler)
        {
        }
    }

    public class RepositoryContainer<TDbContext> : IRepositoryContainer<TDbContext> where TDbContext : DbContext
    {
        public Func<TDbContext, bool, IRepository<TEntity>> ResolveCreateRepository<TEntity>() where TEntity : class =>
            (dContext, noTracking) => new Repository<TEntity>(dContext, noTracking);
    }

    public class OrmContainer<TDbContext> : IOrmContainer<TDbContext> where TDbContext : DbContext
    {
        public Func<TDbContext, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ResolveCreateOrmMetaAdapter<TEntity>() where TEntity : class =>
            (dContext, ormEntitySchemaAdapter) => new OrmEntitySchemaAdapter<TEntity>(dContext, ormEntitySchemaAdapter);

        public Func<TDbContext, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ResolveCreateOrmStorage<TEntity>() where TEntity : class =>
            (dContext, analyzeException, auditVisitor) => new OrmStorage<TEntity>(dContext, analyzeException, auditVisitor);
    }

    public class OrmEntitySchemaAdapter<TEntity> : IOrmEntitySchemaAdapter<TEntity> where TEntity : class
    {
        private readonly DbContext model;
        private readonly IOrmEntitySchemaAdapter ormEntitySchemaAdapter;

        public OrmEntitySchemaAdapter(DbContext model, IOrmEntitySchemaAdapter ormEntitySchemaAdapter)
        {
            this.model = model;
            this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
        }

        public Include<TEntity> AppendModelFields(Include<TEntity> include) =>
            throw new NotImplementedException(nameof(AppendModelFields));
            //EfCoreExtensions.AppendModelFields(include, model);

        public Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include) =>
            throw new NotImplementedException(nameof(AppendModelFieldsIfEmpty));
        //EfCoreExtensions.AppendModelFieldsIfEmpty(include, model);

        public Include<TEntity> ExtractNavigations(Include<TEntity> include) =>
            throw new NotImplementedException(nameof(ExtractNavigations));
        //EfCoreExtensions.ExtractNavigations(include, model);

        public Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include) =>
            throw new NotImplementedException(nameof(ExtractNavigationsAppendKeyLeafs));
        //EfCoreExtensions.ExtractNavigationsAppendKeyLeafs(include, model);

        #region IOrmEntitySchemaAdapter
        public string[] GetBinaries()
        {
            return ormEntitySchemaAdapter.GetBinaries();
        }

        public (string[] Attributes, string Message) GetConstraint(string name)
        {
            return ormEntitySchemaAdapter.GetConstraint(name);
        }

        public string[] GetKeys()
        {
            return ormEntitySchemaAdapter.GetKeys();
        }

        public (string SchemaName, string TableName) GetTableName()
        {
            return ormEntitySchemaAdapter.GetTableName();
        }

        public string[] GetUnique(string name)
        {
            return ormEntitySchemaAdapter.GetUnique(name);
        }

        public string[] GetRequireds()
        {
            return ormEntitySchemaAdapter.GetRequireds();
        }
        #endregion
    }
}
