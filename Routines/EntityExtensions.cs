using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines
{
    public static class EntityExtensions
    {
        public static void UpdateCollection<TRelationEntity>(
                    ICollection<TRelationEntity> oldRelations,
                    IEnumerable<TRelationEntity> newRelations,
                    Func<TRelationEntity, TRelationEntity, bool> equalsById,
                    Action<TRelationEntity> setAuditProperties = null)
        {
            var tmp = new List<TRelationEntity>();
            foreach (var e in oldRelations)
                if (!newRelations.Any(e2 => equalsById(e, e2)))
                    tmp.Add(e);
            foreach (var e in tmp)
                oldRelations.Remove(e);
            foreach (var e in newRelations)
                if (!oldRelations.Any(e2 => equalsById(e, e2)))
                {
                    setAuditProperties?.Invoke(e);
                    oldRelations.Add(e);
                }
        }

        public static void UpdateCollection<TRelationEntity>(
                    ICollection<TRelationEntity> oldRelations,
                    IEnumerable<TRelationEntity> newRelations,
                    Func<TRelationEntity, TRelationEntity, bool> equalsById,
                    Func<TRelationEntity, TRelationEntity, bool> equalsByValue,
                    Action<TRelationEntity, TRelationEntity> updateValue,
                    Action<TRelationEntity> setAuditProperties = null)
        {
            var forRemove = new List<TRelationEntity>();
            foreach (var e in oldRelations)
                if (!newRelations.Any(e2 => equalsById(e, e2)))
                    forRemove.Add(e);
            foreach (var e in forRemove)
                oldRelations.Remove(e);
            foreach (var e in newRelations) {
                var existed = oldRelations.Where(e2 => equalsById(e, e2)).SingleOrDefault();
                if (existed==null)
                {
                    setAuditProperties?.Invoke(e);
                    oldRelations.Add(e);
                }
                else
                {
                    if (!equalsByValue(e, existed))
                        updateValue(e, existed);
                }
            }
        }
    }
}