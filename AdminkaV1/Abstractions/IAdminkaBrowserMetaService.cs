using System;
using System.Linq.Expressions;

namespace DashboardCode.AdminkaV1
{
    public interface IAdminkaBrowserMetaService
    {
        int GetLength<TEntity>(Expression<Func<TEntity, string>> getProperty);
    }
}
