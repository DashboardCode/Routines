using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfCoreTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EfCoreTest"].ConnectionString;
            Console.WriteLine("Check connection string:");
            Console.WriteLine(connectionString);
            TestIsland.Reset(connectionString);
            var loggerProvider = new MyLoggerProvider();
            var messages = new List<string>();
            loggerProvider.Verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };
            using (var dbContext = new MyDbContext(connectionString, loggerProvider))
            {
                var parentRecord = dbContext.ParentRecords
                    .Include(e=>e.ParentRecordHierarchyRecordMap)
                    .ThenInclude(e=>e.HierarchyRecord).First(e => e.FieldA == "1_A");
                var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2).ToList();
                var count2 = only2.Count(); // 2
                foreach (var map in only2)
                {
                    dbContext.Entry(map).State = EntityState.Detached;
                    //parentRecord.ParentRecordHierarchyRecordMap.Remove(map);
                }

                EntityEntry<ParentRecord> entry = dbContext.Entry(parentRecord);
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
                    .Include(e => e.ParentRecordHierarchyRecordMap)
                    .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");

                var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();

                if (count3 != 2)
                    throw new ApplicationException("Tracking error");
            }
        }
    }
}
