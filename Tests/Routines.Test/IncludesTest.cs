using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vse.Routines.Storage;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Vse.Routines.Test
{
    [TestClass]
    public class IncludesTest
    {
        private class TestChild
        {
            public List<Unique> Uniques { get; set; }
        }
        private class TestModel
        {
            public StorageModel StorageModel { get; set; }
            public int[] Test { get; set; }
            public IEnumerable<Guid> ListTest { get; set; }
            public IEnumerable<TestChild> TestChilds { get; set; }
            public ICollection<CultureInfo> CultureInfos { get; set; }

            public string PropertyText { get; set; }
            public int PropertyInt { get; set; }
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
                PropertyText = "sampleTest",
                PropertyInt = 1234
            };
            source.TestChilds = new HashSet<TestChild>() { new TestChild { Uniques = source.StorageModel.Uniques.ToList() } };
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
        public void IncludesClone2Test()
        {
            var source = CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i=>i.ListTest);
            var destination = MemberExpressionExtensions.Clone(source, includes);
            var b1 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b1 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");

            Include<TestModel> includes2
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                            .ThenInclude(i => i.IndexName) // compare
                    .Include(i => i.ListTest);
            var b2 = MemberExpressionExtensions.Equals(source, destination, includes2);
            if (b2 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");
        }

        [TestMethod]
        public void IncludesClone5Test()
        {
            var source = CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques);
            var destination = MemberExpressionExtensions.Clone(source, includes);
            //var b1 = MemberExpressionExtensions.Equals(source, destination, includes);
            //if (b1 == true)
            //    throw new ApplicationException("Eqauls doesn't working properly. Case 0");

            //Include<TestModel> includes2
            //    = includable => includable
            //        .IncludeAll(i => i.TestChilds)
            //            .ThenIncludeAll(i => i.Uniques)
            //                .ThenInclude(i => i.IndexName) // compare
            //        .Include(i => i.ListTest);
            //var b2 = MemberExpressionExtensions.Equals(source, destination, includes2);
            //if (b2 == false)
            //    throw new ApplicationException("Eqauls doesn't working properly. Case 1");
        }

        [TestMethod]
        public void IncludesClone3Test()
        {
            var source = CreateTestModel();
            source.TestChilds = null;
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i => i.ListTest);
            var destination = MemberExpressionExtensions.Clone(source, includes);
            var b1 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");
        }
        public void IncludesClone4Test()
        {
            var source = CreateTestModel();
            foreach(var t in source.TestChilds)
                t.Uniques = null;
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i => i.ListTest);
            var destination = MemberExpressionExtensions.Clone(source, includes);
            var b1 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");
        }

        [TestMethod]
        public void IncludesCloneTest()
        {

            var source = CreateTestModel();
            var includes = CreateIncludes();

            var destination = MemberExpressionExtensions.Clone(source, includes, MemberExpressionExtensions.SystemTypes);

            if (source.PropertyInt!=destination.PropertyInt 
                ||
            source.PropertyText != destination.PropertyText)
                throw new ApplicationException("Copy doesn't working properly. Case 0");

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
