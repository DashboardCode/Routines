using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using DashboardCode.Routines.Storage.EfModelTest;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Ef6.Sandbox
{
    public class StraightEfTests
    {
        public static void TestHierarchy(MyDbContext dbContext)
        {
            var parentRecord = dbContext.ParentRecords
                    .Include("ParentRecordHierarchyRecordMap")
                    .Include("ParentRecordHierarchyRecordMap.HierarchyRecord").First(e => e.FieldA == "1_A");
            var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
            var only2  = parentRecord.ParentRecordHierarchyRecordMap.Take(2).ToList();
            var count2 = only2.Count(); // 2
            foreach (var map in only2)
            {
                dbContext.Entry(map).State = EntityState.Detached;
                //parentRecord.ParentRecordHierarchyRecordMap.Remove(map);
            }

            DbEntityEntry<ParentRecord> entry = dbContext.Entry(parentRecord);
            var col = entry.Collection(e => e.ParentRecordHierarchyRecordMap);
            col.Load();
            var oldRelations = parentRecord.ParentRecordHierarchyRecordMap;
            var tmp = new List<ParentRecordHierarchyRecord>();
            foreach (var e1 in oldRelations)
                if (!only2.Any(e2 => e1.HierarchyRecordId == e2.HierarchyRecordId))
                    tmp.Add(e1);
            foreach (var e in tmp)
                oldRelations.Remove(e);

            var count2b = parentRecord.ParentRecordHierarchyRecordMap.Count();
            //foreach (var e1 in only2)
            //    if (!oldRelations.Any(e2 => e1.HierarchyRecordId == e2.HierarchyRecordId))
            //        oldRelations.Add(e1);

            dbContext.SaveChanges();

            var parentRecord2 = dbContext.ParentRecords
                .Include("ParentRecordHierarchyRecordMap")
                .Include("ParentRecordHierarchyRecordMap.HierarchyRecord").First(e => e.FieldA == "1_A");

            var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();

            if (count3 != 2)
                throw new ApplicationException("Tracking error");
        }
    }
}