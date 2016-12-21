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

            private string Name2 { get; set; }

            public string this[int index]
            {
                get
                {
                    return Name2[index].ToString();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public MessageStruct Message {get;set;}
            public struct MessageStruct{
                public string TextMsg { get; set; }
                public DateTime DateTimeMsg { get; set; }
                public int? IntNullableMsg { get; set; }
            }

            public int? IntNullable1 { get; set; }
            public int? IntNullable2 { get; set; }
        }
        private  TestModel CreateTestModel()
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
            source.Message = new TestModel.MessageStruct() { TextMsg = "Initial", DateTimeMsg = DateTime.Now, IntNullableMsg = 7 };
            source.IntNullable2 = 555;
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
                             .ThenIncludeAll(i => i.Fields)
                    .Include(i => i.Message)
                         .ThenInclude(i => i.TextMsg)
                    .Include(i => i.Message)
                         .ThenInclude(i => i.DateTimeMsg)
                    .Include(i => i.Message)
                         .ThenInclude(i => i.IntNullableMsg)
                    .Include(i => i.IntNullable1)
                    .Include(i => i.IntNullable2);
            return includes;
        }

        [TestMethod]
        public void IncludesCloneAll()
        {
            var list = new List<TestModel>();
            list.Add(CreateTestModel());
            list.Add(CreateTestModel());
            list.Add(CreateTestModel());

            var includes = CreateIncludes();

            var cloned = MemberExpressionExtensions.CloneAll(list, includes);
            

            // default include contain key function; expected true
            var equals = MemberExpressionExtensions.EqualsAll(list, cloned, includes);
            if (!equals)
                throw new ApplicationException("IncludesCloneAll error 0");

            // no includes = no key function; expected false
            var equals1 = MemberExpressionExtensions.EqualsAll<List<TestModel>, TestModel>(list, cloned);
            if (equals1)
                throw new ApplicationException("IncludesCloneAll error 1");

            cloned[0].StorageModel.Uniques[0].Fields[0] = "changed";

            var equals2 = MemberExpressionExtensions.EqualsAll(list, cloned, includes);
            if (equals2)
                throw new ApplicationException("IncludesCloneAll error 2");

            // for coverage
            var clonedB = MemberExpressionExtensions.CloneAll(list, includes, MemberExpressionExtensions.SystemTypes); 
            var clonedNull = MemberExpressionExtensions.Clone(default(TestModel), includes, MemberExpressionExtensions.SystemTypes);
            var clonedNulls = MemberExpressionExtensions.CloneAll<List<TestModel>,TestModel>(null, includes);
            var xx = new List<TestModel>();
            MemberExpressionExtensions.CopyAll<List<TestModel>, TestModel>(list, xx);

        }

        [TestMethod]
        public void IncludesDetach()
        {
            var source = CreateTestModel();
            var includes = CreateIncludes();
            MemberExpressionExtensions.Detach(source, includes);

            if (source.CultureInfos!=null)
               throw new ApplicationException("Detach doesn't working properly");
        }

        [TestMethod]
        public void IncludesPathes()
        {
            var source = CreateTestModel();
            var includes = CreateIncludes();
            var including = new MemberExpressionExtensions.PathesIncluding<TestModel>();
            var includable = new Includable<TestModel>(including);
            includes.Invoke(includable);
            var pathes = including.Pathes;

            if (pathes.Count != 13)
                throw new ApplicationException("PathesIncluding doesn't working properly");
        }

        [TestMethod]
        public void IncludesCopyTest()
        {

            var source = CreateTestModel();
            var destination = new TestModel();
            var includes = CreateIncludes();
            MemberExpressionExtensions.Copy(source, destination, includes);

            if (source.StorageModel.Entity.Name != destination.StorageModel.Entity.Name
                || source.StorageModel.Entity.Namespace != destination.StorageModel.Entity.Namespace || source.StorageModel.Key == null)
                throw new ApplicationException("Copy doesn't working properly");
        }
        [TestMethod]
        public void IncludesEqualsTest()
        {
            
            var source = CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i=>i.ListTest);
            var destination = MemberExpressionExtensions.Clone(source, includes);

            //equals by reference will be false
            var b1 = MemberExpressionExtensions.Equals(source, destination, includes);
            if (b1 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");

            //equals by field value will be true
            Include<TestModel> equalsIncludes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                            .ThenInclude(i => i.IndexName) // compare
                    .Include(i => i.ListTest);
            var b2 = MemberExpressionExtensions.Equals(source, destination, equalsIncludes);
            if (b2 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");

            foreach (var c in destination.TestChilds)
                c.Uniques[0].IndexName = null;

            if (MemberExpressionExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 2");

            foreach (var c in source.TestChilds)
                c.Uniques[0].IndexName = null;

            if (!MemberExpressionExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 3");

            foreach (var c in destination.TestChilds)
                c.Uniques[0].IndexName = "notnull";
            if (MemberExpressionExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 2b");

            // equalsIncludes correct,  into clone key is not included neither by include, neither by types; expected false
            var source2 = CreateTestModel();
            var destination2 = MemberExpressionExtensions.Clone(source2, includes, new List<Type>());
            if (MemberExpressionExtensions.Equals(source2, destination2, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 4");

            // equalsIncludes correct,  into clone key is included by types, but not by clone Include; expected true
            var cloned3 = MemberExpressionExtensions.Clone(source2, includes);
            if (!MemberExpressionExtensions.Equals(source2, cloned3, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 5");
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
        public void IncludesGetTypes()
        {
            var source = CreateTestModel();
            var includes = CreateIncludes();

            var b1 = MemberExpressionExtensions.GetTypes(includes);
            if (b1.Count() != 11)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");
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

        [TestMethod]
        public void IncludesEquals()
        {
            int[] e1 = new int[0];
            int[] e2 = new int[1] {7};
            int[] e3 = new int[1] {7};

            var x1 = MemberExpressionExtensions.Equals(e3, e2, null);
            var x2 = MemberExpressionExtensions.Equals(e1, e2, null);
            if (x1 != true || x2 != false)
                throw new ApplicationException("Test Failed. Case 0");

            var x3 = MemberExpressionExtensions.Equals(e3.ToList(), e2.ToList(), null);
            var x4 = MemberExpressionExtensions.Equals(e1.ToList(), e2.ToList(), null);
            if (x3 != true || x4 != false)
                throw new ApplicationException("Test Failed. Case 1");

            int[] e4 = new int[1];
            MemberExpressionExtensions.Copy(e2, e4, null);
            if (e4[0]!=e2[0])
                throw new ApplicationException("Test Failed. Case 2");

            try
            {
                MemberExpressionExtensions.Copy(e2, e1, null);
            }
            catch (InvalidOperationException)
            {
                
            }

            var items = new List<Item>();
            items.Add(null);
            items.Add(null);
            items.Add(new Item() { F1 = "F1", F2 = "F2", Items = items });
            MemberExpressionExtensions.DetachAll<List<Item>, Item>(items, (i)=>i.Include(e=>e.Items));
        }

        public class Item
        {
            public string F1 { get; set; }
            public string F2 { get; set; }

            public List<Item> Items {get;set;}
        }
    }
}
