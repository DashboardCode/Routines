using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.TestDom;

namespace DashboardCode.AdminkaV1.Injected.InMemory.Test
{
    class InjectedStreamSerializationsTest : RepositoryTestBase
    {
        [TestMethod]
        public virtual void TestSerializtionRecursion()
        {
            var logger = new List<string>();
            var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { }, readonlyDatabaseName);
            var record = routine.Handle((container, closure) => container.ResolveAdminkaDbContextHandler().HandleOrmFactory(ormHandlerFactory =>
            {
                Include<TypeRecord> include = includable =>
                       includable.IncludeAll(y => y.ChildRecords)
                       .ThenInclude(y => y.TypeRecord);

                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                return repositoryHandler.Handle((repository, storage) =>
                {
                    return repository.Find(e => e.TestTypeRecordId == "0000", include);
                });
            }));
            var json = InjectedManager.SerializeToJson(record, 2, true);
        }


        //[TestMethod]
        //public virtual void TestXmlSerializeAndDesirialize()
        //{
        //    var logger = new List<string>();
        //    var routine = new AdminkaInMemoryTestRoutine(logger, new MemberTag(this), new { }, readonlyDatabaseName);
        //    Include<TypeRecord> include = includable =>
        //               includable.IncludeAll(y => y.ChildRecords)
        //               .ThenInclude(y => y.TypeRecord);
        //    var record = routine.HandleOrmFactory((ormHandlerFactory) =>
        //    {
        //        var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
        //        return repositoryHandler.Handle((repository, storage) =>
        //        {
        //            var entity = repository.Find(e => e.TestTypeRecordId == "0000", include);
        //            return entity;
        //        });
        //    });
        //    var cloned = ObjectExtensions.Clone(record, include, SystemTypesExtensions.SystemTypes);
        //    if (cloned.ChildRecords == null || cloned.ChildRecords.Count == 0)
        //        throw new Exception("Clone error");
        //    var xml = InjectedManager.SerializeToXml(cloned, include);
        //    var o = InjectedManager.DeserializeXml(xml, include);
        //    if (o == null)
        //        throw new Exception("Serialize error");
        //}
    }
}