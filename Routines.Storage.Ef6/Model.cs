using System.Data.Entity;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class Model<TEntity> :  IModel<TEntity> where TEntity : class
    {
        private readonly DbContext context;

        public Model(DbContext context) =>
            this.context = context;

        public Include<TEntity> AppendModelFields(Include<TEntity> include) =>
            Ef6Extensions.AppendModelProperties(include, context);

        public Include<TEntity> AppendModelFieldsIfEmpty(Include<TEntity> include) =>
            Ef6Extensions.AppendModelPropertiesIfEmpty(include, context);

        public Include<TEntity> ExtractNavigations(Include<TEntity> include) =>
            Ef6Extensions.ExtractNavigations(include, context);

        public Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include) =>
            Ef6Extensions.ExtractNavigationsAppendKeyProperties(include, context);

    }
}
