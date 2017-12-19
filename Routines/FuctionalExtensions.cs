using System;

namespace DashboardCode.Routines
{
    public static class FunctionalExtensions
    {
        public static Func<TEntity, Action<TValue>> CombineSetterAndConverter<TEntity, TValue, TPropertyType>(Action<TEntity, TPropertyType> setter, Func<TValue, TPropertyType> converter)
        {
            Func<TEntity, Action<TValue>> func = (e) => v=> setter(e, converter(v));
            return func;
        }
    }
}