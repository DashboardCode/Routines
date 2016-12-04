using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.Routines.Storage;
using System.Collections.Generic;
using System.Globalization;

namespace Vse.Routines.Test
{
    [TestClass]
    public class IncludesTest
    {
        private class TestModel
        {
            public StorageModel StorageModel { get; set; }
            public int[] Test { get; set; }
            public List<Guid> ListTest { get; set; }
            public List<CultureInfo> CultureInfos { get; set; }
        }
        private static TestModel CreateTestModel()
        {
            var source = new TestModel()
            {
                StorageModel = new StorageModel()
                {
                    TableName = "TableName1",
                    Entity = new Entity() { Name = "EntityName1", Namespace = "EntityNamespace1" },
                    Key = new Key() { Attributes = new[] { "FieldA1", "FieldA2" } },
                    Uniques = new[] { new Unique { Fields = new[] { "FieldU1" }, IndexName = "IndexName1" }, new Unique { Fields = new[] { "FieldU2" }, IndexName = "IndexName2" } }
                    
                },
                Test = new[] { 1, 2, 3 },
                ListTest = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() },
                CultureInfos = new List<CultureInfo>() { CultureInfo.CurrentCulture, CultureInfo.InvariantCulture },
            };
            return source;
        }
        private static Include<TestModel> CreateIncludes()
        {
            Include<TestModel> includes
                = includable => includable
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Name)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.TableName)
                    .Include(i => i.Test)
                    .Include(i => i.ListTest)
                    .Include(i => i.StorageModel)
                         .ThenIncludeAll(i => i.Uniques)
                             .ThenInclude(i => i.IndexName)
                    .Include(i => i.StorageModel)
                         .ThenIncludeAll(i => i.Uniques)
                             .ThenIncludeAll(i => i.Fields);
            return includes;
        }

        [TestMethod]
        public void IncludesCopyTest()
        {

            var source = CreateTestModel();
            var destination = new TestModel();
            var includes = CreateIncludes();
            MemberExpressionExtensions.CopyTo(source, destination, includes);

            if (source.StorageModel.Entity.Name != destination.StorageModel.Entity.Name
                || source.StorageModel.Entity.Namespace != destination.StorageModel.Entity.Namespace || source.StorageModel.Key == null)
                throw new ApplicationException("Copy doesn't working properly");
        }
        [TestMethod]
        public void IncludesCloneTest()
        {

            var source = CreateTestModel();
            var includes = CreateIncludes();

            var destination = MemberExpressionExtensions.Clone(source, includes);

            if (source.StorageModel.Entity.Name != destination.StorageModel.Entity.Name
                || source.StorageModel.Entity.Namespace != destination.StorageModel.Entity.Namespace || source.StorageModel.Key == null)
                throw new ApplicationException("Copy doesn't working properly");

            var b1 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");

            source.Test[2] = 4;
            var b2 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b2 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 2");

            destination.Test[2] = 4;
            var b3 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b3 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 3");

            source.StorageModel.Key.Attributes[1] = "Field3";
            var b4 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b4 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 4");

            destination.StorageModel.Key.Attributes[1] = "Field3";
            var b5 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b5 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 5");

        }
        [TestMethod]
        public void IncludesEF6Style()
        {
            var source = CreateTestModel();
            var includes = CreateIncludes();

            var including = new MemberExpressionExtensions.PathesIncluding<TestModel>();
            includes?.Invoke(new Includable<TestModel>(including));
            var ef6Includes = including.Pathes.ConvertAll(e => string.Join(".", e));
        }
    }
}
