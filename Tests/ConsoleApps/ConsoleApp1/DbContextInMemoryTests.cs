using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public class DbContextInMemoryTests
    {
        internal static void InMemoryTest()
        {
            var connectionString = Guid.NewGuid().ToString();
            using (var dbContext = new MyDbContext(MyDbContext.Build(connectionString)))
            {
                var parentRecord1 = new ParentRecord()
                {
                    FieldA = "1_A",
                    FieldB1 = "1_B",
                    FieldB2 = "1_C",
                    FieldCA = "1_1",
                    FieldCB1 = "1_2",
                    FieldCB2 = "1_3"
                };

                var parentRecord2 = new ParentRecord()
                {
                    FieldA = "2_A",
                    FieldB1 = "2_B",
                    FieldB2 = "2_C",
                    FieldCA = "2_1",
                    FieldCB1 = "2_2",
                    FieldCB2 = "2_3"
                };

                dbContext.ParentRecords.Add(parentRecord1);
                dbContext.ParentRecords.Add(parentRecord2);
                dbContext.SaveChanges();
            }

            int id = 0;
            // ACT1: works well
            using (var dbContext = new MyDbContext(MyDbContext.Build(connectionString, null)))
            {
                var parentRecords = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).ToList();
                var parentRecord = parentRecords.First(e => e.FieldA == "1_A");
                id = parentRecord.ParentRecordId;
            }

            // ACT2: the same + verbose ruins test !
            // throws ': 'Sequence contains no matching element' means can't access data
            var messages = new List<string>();
            Action<string> VERBOSE = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };
            using (var dbContext = new MyDbContext(MyDbContext.Build(connectionString, VERBOSE)))
            {
                var parentRecords = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).ToList();
                var parentRecord = parentRecords.First(e => e.FieldA == "1_A");
                id = parentRecord.ParentRecordId;
            }

        }
    }
}