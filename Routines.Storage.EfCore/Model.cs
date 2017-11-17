using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class Model<TEntity> :  IModel<TEntity> where TEntity : class
    {
        private readonly DbContext context;

        public Model(DbContext context) =>
            this.context = context;

        public Include<TEntity> AppendModelFields(Include<TEntity> include) =>
            EfCoreExtensions.AppendModelFields(include, context);

        public Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include) =>
            EfCoreExtensions.AppendModelFieldsIfEmpty(include, context);

        public Include<TEntity> ExtractNavigations(Include<TEntity> include) =>
            EfCoreExtensions.ExtractNavigations(include, context);

        public Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include) =>
            EfCoreExtensions.ExtractNavigationsAppendKeyLeafs(include, context);

    }
}