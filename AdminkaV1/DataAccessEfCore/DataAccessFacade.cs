using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Storage.EfCore;
using System;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContextWrapper : IDisposable
    {
        public readonly AdminkaDbContext DbContext;
        public AdminkaDbContextWrapper(AdminkaDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }

    public class RepositoryGFactory2 : IRepositoryGFactory<AdminkaDbContextWrapper>
    {
        public Func<AdminkaDbContextWrapper, bool, IRepository<TEntity>> ComposeCreateRepository<TEntity>() where TEntity : class =>
            (daContext, noTracking) => new Repository<TEntity>(daContext.DbContext, noTracking);
    }

    public class OrmGFactory2 : IOrmGFactory<AdminkaDbContextWrapper>
    {
        public Func<AdminkaDbContextWrapper, IOrmEntitySchemaAdapter, IOrmEntitySchemaAdapter<TEntity>> ComposeCreateOrmMetaAdapter<TEntity>() where TEntity : class =>
            (dContext, ormEntitySchemaAdapter) => new OrmMetaAdapter<TEntity>(dContext.DbContext.Model, ormEntitySchemaAdapter);

        public Func<AdminkaDbContextWrapper, Func<Exception, StorageResult>, IAuditVisitor, IOrmStorage<TEntity>> ComposeCreateOrmStorage<TEntity>() where TEntity : class =>
            (dContext, analyzeException, auditVisitor) => new OrmStorage<TEntity>(dContext.DbContext, analyzeException, auditVisitor);
    }

    public class AdminkaDataAccessFacade2 : DataAccessFacade<UserContext, AdminkaDbContextWrapper>
    {
        public AdminkaDataAccessFacade2(
                IStorageMetaService storageMetaService,
                Func<RoutineClosure<UserContext>, AdminkaDbContextWrapper> dbContextFactory
            ) : base(
                        new RepositoryGFactory2(),
                        new OrmGFactory2(),
                        storageMetaService,
                        (closure) => new AuditVisitor(closure.UserContext),
                        dbContextFactory
            )
        {
        }
    }
}
