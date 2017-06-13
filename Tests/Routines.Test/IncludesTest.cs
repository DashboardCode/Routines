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
        
        

        [TestMethod]
        public void IncludesCloneAll()
        {
            var list = new List<TestModel>() { TestTool.CreateTestModel() , TestTool.CreateTestModel(), TestTool.CreateTestModel() };

            var includes = TestTool.CreateInclude();

            var cloned = ChainExtensions.CloneAll(list, includes);
            

            // default include contain key function; expected true
            var equals = ChainExtensions.EqualsAll(list, cloned, includes);
            if (!equals)
                throw new ApplicationException("IncludesCloneAll error 0");

            // no includes = no key function; expected false
            var equals1 = ChainExtensions.EqualsAll<List<TestModel>, TestModel>(list, cloned);
            if (equals1)
                throw new ApplicationException("IncludesCloneAll error 1");

            cloned[0].StorageModel.Uniques[0].Fields[0] = "changed";

            var equals2 = ChainExtensions.EqualsAll(list, cloned, includes);
            if (equals2)
                throw new ApplicationException("IncludesCloneAll error 2");

            // for coverage
            var clonedB = ChainExtensions.CloneAll(list, includes, SystemTypesExtensions.SystemTypes); 
            var clonedNull = ChainExtensions.Clone(default(TestModel), includes, SystemTypesExtensions.SystemTypes);
            var clonedNulls = ChainExtensions.CloneAll<List<TestModel>,TestModel>(null, includes);
            var xx = new List<TestModel>();
            ChainExtensions.CopyAll<List<TestModel>, TestModel>(list, xx);

        }

        [TestMethod]
        public void IncludesDetach()
        {
            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();
            ChainExtensions.Detach(source, includes);

            if (source.CultureInfos!=null)
               throw new ApplicationException("Detach doesn't working properly");
        }

        [TestMethod]
        public void IncludesPathes()
        {
            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();
            var including = new PathesChainParser<TestModel>();
            var includable = new Includable<TestModel>(including);
            includes.Invoke(includable);
            var pathes = including.Pathes;

            if (pathes.Count != 13)
                throw new ApplicationException("PathesIncluding doesn't working properly");
        }

        [TestMethod]
        public void IncludesContainsTest()
        {

            var source = TestTool.CreateTestModel();
            var destination = new TestModel();
            var include = TestTool.CreateInclude();
            Include<TestModel> include1 
                = includable => includable
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Name);

            var contains1 = include.Contains(include1);

            if (!contains1)
                throw new ApplicationException("Contains (1)");

            Include<TestModel> include2
                = includable => includable
                    .Include(i => i.CultureInfos)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Name);

            var contains2 = include.Contains(include2);

            if (contains2)
                throw new ApplicationException("Contains (2)");
        }

        [TestMethod]
        public void IncludesUnionTest()
        {
            Include<TestModel> include1
                = includable => includable
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes);

            Include<TestModel> include2
                = includable => includable
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Name);

            Include<TestModel> include3
                = includable => includable
                    .Include(i => i.CultureInfos);

            var include = include1.Union(include2);

            if (!(include.Contains(include1) && include.Contains(include2)))
                throw new ApplicationException("IncludesUnionTest 1");

            if (include.Contains(include3))
                throw new ApplicationException("IncludesUnionTest 2");
        }

        [TestMethod]
        public void IncludesCopyTest()
        {

            var source = TestTool.CreateTestModel();
            var destination = new TestModel();
            var includes = TestTool.CreateInclude();
            ChainExtensions.Copy(source, destination, includes);

            if (source.StorageModel.Entity.Name != destination.StorageModel.Entity.Name
                || source.StorageModel.Entity.Namespace != destination.StorageModel.Entity.Namespace || source.StorageModel.Key == null)
                throw new ApplicationException("Copy doesn't working properly");
        }
        [TestMethod]
        public void IncludesEqualsTest()
        {
            
            var source = TestTool.CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i=>i.ListTest);
            var destination = ChainExtensions.Clone(source, includes);

            //equals by reference will be false
            var b1 = ChainExtensions.Equals(source, destination, includes);
            if (b1 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");

            //equals by field value will be true
            Include<TestModel> equalsIncludes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                            .ThenInclude(i => i.IndexName) // compare
                    .Include(i => i.ListTest);
            var b2 = ChainExtensions.Equals(source, destination, equalsIncludes);
            if (b2 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");

            foreach (var c in destination.TestChilds)
                c.Uniques[0].IndexName = null;

            if (ChainExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 2");

            foreach (var c in source.TestChilds)
                c.Uniques[0].IndexName = null;

            if (!ChainExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 3");

            foreach (var c in destination.TestChilds)
                c.Uniques[0].IndexName = "notnull";
            if (ChainExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 2b");

            // equalsIncludes correct,  into clone key is not included neither by include, neither by types; expected false
            var source2 = TestTool.CreateTestModel();
            var destination2 = ChainExtensions.Clone(source2, includes, new List<Type>());
            if (ChainExtensions.Equals(source2, destination2, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 4");

            // equalsIncludes correct,  into clone key is included by types, but not by clone Include; expected true
            var cloned3 = ChainExtensions.Clone(source2, includes);
            if (!ChainExtensions.Equals(source2, cloned3, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 5");
        }

        [TestMethod]
        public void IncludesClone5Test()
        {
            var source = TestTool.CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques);
            var destination = ChainExtensions.Clone(source, includes);
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
            var source = TestTool.CreateTestModel();
            source.TestChilds = null;
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i => i.ListTest);
            var destination = ChainExtensions.Clone(source, includes);
            var b1 = ChainExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");
        }
        public void IncludesClone4Test()
        {
            var source = TestTool.CreateTestModel();
            foreach(var t in source.TestChilds)
                t.Uniques = null;
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                    .Include(i => i.ListTest);
            var destination = ChainExtensions.Clone(source, includes);
            var b1 = ChainExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");
        }

        [TestMethod]
        public void IncludesGetTypes()
        {
            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();

            var b1 = ChainExtensions.GetTypes(includes);
            if (b1.Count() != 11)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");
        }

        [TestMethod]
        public void IncludesCloneTest()
        {

            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();

            var destination = ChainExtensions.Clone(source, includes, SystemTypesExtensions.SystemTypes);

            if (source.PropertyInt!=destination.PropertyInt 
                ||
            source.PropertyText != destination.PropertyText)
                throw new ApplicationException("Copy doesn't working properly. Case 0");

            if (source.StorageModel.Entity.Name != destination.StorageModel.Entity.Name
                || source.StorageModel.Entity.Namespace != destination.StorageModel.Entity.Namespace || source.StorageModel.Key == null)
                throw new ApplicationException("Copy doesn't working properly");

            var b1 = ChainExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");

            source.Test[2] = 4;
            var b2 = ChainExtensions.Equals(source, destination, includes);
            if (b2 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 2");

            destination.Test[2] = 4;
            var b3 = ChainExtensions.Equals(source, destination, includes);
            if (b3 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 3");

            source.StorageModel.Key.Attributes[1] = "Field3";
            var b4 = ChainExtensions.Equals(source, destination, includes);
            if (b4 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 4");

            destination.StorageModel.Key.Attributes[1] = "Field3";
            var b5 = ChainExtensions.Equals(source, destination, includes);
            if (b5 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 5");

        }
        [TestMethod]
        public void IncludesEF6Style()
        {
            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();

            var including = new PathesChainParser<TestModel>();
            includes?.Invoke(new Includable<TestModel>(including));
            var ef6Includes = including.Pathes.ConvertAll(e => string.Join(".", e));
        }

        [TestMethod]
        public void IncludesEquals()
        {
            int[] e1 = new int[0];
            int[] e2 = new int[1] {7};
            int[] e3 = new int[1] {7};

            var x1 = ChainExtensions.Equals(e3, e2, null);
            var x2 = ChainExtensions.Equals(e1, e2, null);
            if (x1 != true || x2 != false)
                throw new ApplicationException("Test Failed. Case 0");

            var x3 = ChainExtensions.Equals(e3.ToList(), e2.ToList(), null);
            var x4 = ChainExtensions.Equals(e1.ToList(), e2.ToList(), null);
            if (x3 != true || x4 != false)
                throw new ApplicationException("Test Failed. Case 1");

            int[] e4 = new int[1];
            ChainExtensions.Copy(e2, e4, null);
            if (e4[0]!=e2[0])
                throw new ApplicationException("Test Failed. Case 2");

            try
            {
                ChainExtensions.Copy(e2, e1, null);
            }
            catch (InvalidOperationException)
            {
                
            }

            var items = new List<Item>() { null, null};
            items.Add(new Item() { F1 = "F1", F2 = "F2", Items = items });
            ChainExtensions.DetachAll<List<Item>, Item>(items, (i)=>i.Include(e=>e.Items));
        }
    }
}
