using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.Routines.Storage;
using System.Collections.Generic;
using System.Linq;
using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.Test
{
    [TestClass]
    public class IncludesTest
    {
        [TestMethod]
        public void IncludesCloneIncludeTest()
        {
            Include<TestModel> include1 = chain => chain
                    .Include(e => e.IntNullable1)
                    .Include(e => e.IntNullable1)
                    .IncludeAll(e => e.TestChilds).ThenIncludeAll(e => e.Uniques).ThenInclude(e => e.IndexName)
                    .IncludeAll(e => e.TestChilds).ThenIncludeAll(e => e.Uniques).ThenInclude(e => e.Fields);

            var include2 = IncludeExtensions.Clone(include1);

            var b1 = IncludeExtensions.IsEqualTo(include1, include2);
            if (!b1)
                throw new ApplicationException("IncludesCloneIncludeTest not equals");

            Include<TestModel> includeX = t => t.Include(e => e.PropertyInt); //.Include(e => e.PropertyText);
            var b2 = IncludeExtensions.IsEqualTo(include1, includeX);
            if (b2)
                throw new ApplicationException("IncludesCloneIncludeTest false equals");
        }

        [TestMethod]
        public void IncludesAppendLeafsTest()
        {
            Include<TestModel> include1 = chain => chain
                    .IncludeAll(e => e.TestChilds)
                    .ThenIncludeAll(e => e.Uniques);
            var appendedInclude = include1.AppendLeafs(); 
            var pathes1 = appendedInclude.ListLeafKeyPaths();
            var pathes2 = appendedInclude.ListLeafXPaths();
            if (pathes1.Count!=5)
                throw new ApplicationException("IncludesAppendLeafsTest error");
        }

        [TestMethod]
        public void IncludesCloneAll()
        {
            var list = new List<TestModel>() { TestTool.CreateTestModel() , TestTool.CreateTestModel(), TestTool.CreateTestModel() };

            var includes = TestTool.CreateInclude();

            var cloned = ObjectExtensions.CloneAll(list, includes);
            

            // default include contain key function; expected true
            var equals = ObjectExtensions.EqualsAll(list, cloned, includes);
            if (!equals)
                throw new ApplicationException("IncludesCloneAll error 0");

            // no includes = no key function; expected false
            var equals1 = ObjectExtensions.EqualsAll<List<TestModel>, TestModel>(list, cloned);
            if (equals1)
                throw new ApplicationException("IncludesCloneAll error 1");

            cloned[0].StorageModel.Uniques[0].Fields[0] = "changed";

            var equals2 = ObjectExtensions.EqualsAll(list, cloned, includes);
            if (equals2)
                throw new ApplicationException("IncludesCloneAll error 2");

            // for coverage
            var clonedB = ObjectExtensions.CloneAll(list, includes, SystemTypesExtensions.SystemTypes); 
            var clonedNull = ObjectExtensions.Clone(default(TestModel), includes, SystemTypesExtensions.SystemTypes);
            var clonedNulls = ObjectExtensions.CloneAll<List<TestModel>,TestModel>(null, includes);
            var xx = new List<TestModel>();
            ObjectExtensions.CopyAll<List<TestModel>, TestModel>(list, xx);

        }

        [TestMethod]
        public void IncludesDetach()
        {
            var source = TestTool.CreateTestModel();

            var source1 = TestTool.CreateTestModel();
            var include1 = TestTool.CreateInclude();

            ObjectExtensions.Detach(source1, include1);

            var source2 = TestTool.CreateTestModel();
            var include2 = TestTool.CreateInclude();
            var includeWithLeafs = include2.AppendLeafs();

            ObjectExtensions.Detach2(source2, includeWithLeafs);

            var b1 = ObjectExtensions.Equals(source1, source, include1);
            var b2 = ObjectExtensions.Equals(source2, source, include2);

            var b3 = ObjectExtensions.Equals(source1, source2, include1);
            var b4 = ObjectExtensions.Equals(source1, source2, include2);

            //Json.NExpJsonSerializerStringBuilderExtensions.

            if (b1==false || b2 == false || b3 == false || b4 == false)
                throw new ApplicationException("Detach doesn't working properly - wrong equals results");
            if (source1.CultureInfos!=null)
               throw new ApplicationException("Detach doesn't working properly");
        }

        [TestMethod]
        public void IncludesPathes()
        {
            var source = TestTool.CreateTestModel();
            var include = TestTool.CreateInclude();
            var pathes = include.ListLeafKeyPaths();
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

            var contains1 = include.IsSupersetOf(include1);

            if (!contains1)
                throw new ApplicationException("Contains (1)");

            Include<TestModel> include2
                = includable => includable
                    .Include(i => i.CultureInfos)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenIncluding(i => i.Namespace)
                            .ThenInclude(i => i.Name)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes);

            var contains2 = include.IsSupersetOf(include2);

            if (contains2)
                throw new ApplicationException("Contains (2)");
        }

        [TestMethod]
        public void IncludesUnionTest()
        {
            Include<TestModel> include1
                = chain => chain
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes);

            Include<TestModel> include2
                = chain => chain
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenIncluding(i => i.Namespace)
                            .ThenInclude(i => i.Name);
            //.Include(i => i.StorageModel)
            //    .ThenInclude(i => i.Entity)
            //        .ThenInclude(i => i.Name);

            Include<TestModel> include3
                = includable => includable
                    .Include(i => i.CultureInfos);

            var include = include1.Merge(include2);

            if (!(include.IsSupersetOf(include1) && include.IsSupersetOf(include2)))
                throw new ApplicationException("IncludesUnionTest 1");

            if (include.IsSupersetOf(include3))
                throw new ApplicationException("IncludesUnionTest 2");
        }

        [TestMethod]
        public void IncludesCopyTest()
        {

            var source = TestTool.CreateTestModel();
            var destination = new TestModel();
            var includes = TestTool.CreateInclude();
            ObjectExtensions.Copy(source, destination, includes);

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
            var destination = ObjectExtensions.Clone(source, includes);

            //equals by reference will be false
            var b1 = ObjectExtensions.Equals(source, destination, includes);
            if (b1 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");

            //equals by field value will be true
            Include<TestModel> equalsIncludes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                            .ThenInclude(i => i.IndexName) // compare
                    .Include(i => i.ListTest);
            var b2 = ObjectExtensions.Equals(source, destination, equalsIncludes);
            if (b2 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");

            foreach (var c in destination.TestChilds)
                c.Uniques[0].IndexName = null;

            if (ObjectExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 2");

            foreach (var c in source.TestChilds)
                c.Uniques[0].IndexName = null;

            if (!ObjectExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 3");

            foreach (var c in destination.TestChilds)
                c.Uniques[0].IndexName = "notnull";
            if (ObjectExtensions.Equals(source, destination, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 2b");

            // equalsIncludes correct,  into clone key is not included neither by include, neither by types; expected false
            var source2 = TestTool.CreateTestModel();
            var destination2 = ObjectExtensions.Clone(source2, includes, new List<Type>());
            if (ObjectExtensions.Equals(source2, destination2, equalsIncludes))
                throw new ApplicationException("Eqauls doesn't working properly. Case 4");

            // equalsIncludes correct,  into clone key is included by types, but not by clone Include; expected true
            var cloned3 = ObjectExtensions.Clone(source2, includes);
            if (!ObjectExtensions.Equals(source2, cloned3, equalsIncludes))
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
            var destination = ObjectExtensions.Clone(source, includes);
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
            var destination = ObjectExtensions.Clone(source, includes);
            var b1 = ObjectExtensions.Equals(source, destination, includes);
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
            var destination = ObjectExtensions.Clone(source, includes);
            var b1 = ObjectExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 0");
        }

        [TestMethod]
        public void IncludesGetTypes()
        {
            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();

            var b1 = IncludeExtensions.ListLeafTypes(includes);
            if (b1.Count() != 11)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");
        }

        [TestMethod]
        public void IncludesCloneTest()
        {

            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();

            var destination = ObjectExtensions.Clone(source, includes, SystemTypesExtensions.SystemTypes);

            if (source.PropertyInt!=destination.PropertyInt 
                ||
            source.PropertyText != destination.PropertyText)
                throw new ApplicationException("Copy doesn't working properly. Case 0");

            if (source.StorageModel.Entity.Name != destination.StorageModel.Entity.Name
                || source.StorageModel.Entity.Namespace != destination.StorageModel.Entity.Namespace || source.StorageModel.Key == null)
                throw new ApplicationException("Copy doesn't working properly");

            var b1 = ObjectExtensions.Equals(source, destination, includes);
            if (b1 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 1");

            source.Test[2] = 4;
            var b2 = ObjectExtensions.Equals(source, destination, includes);
            if (b2 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 2");

            destination.Test[2] = 4;
            var b3 = ObjectExtensions.Equals(source, destination, includes);
            if (b3 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 3");

            source.StorageModel.Key.Attributes[1] = "Field3";
            var b4 = ObjectExtensions.Equals(source, destination, includes);
            if (b4 == true)
                throw new ApplicationException("Eqauls doesn't working properly. Case 4");

            destination.StorageModel.Key.Attributes[1] = "Field3";
            var b5 = ObjectExtensions.Equals(source, destination, includes);
            if (b5 == false)
                throw new ApplicationException("Eqauls doesn't working properly. Case 5");

        }
        [TestMethod]
        public void IncludesEF6Style()
        {
            var source = TestTool.CreateTestModel();
            var includes = TestTool.CreateInclude();
            var paths = includes.ListLeafKeyPaths();
            var ef6Includes = paths.Select(e => string.Join(".", e));
        }

        [TestMethod]
        public void IncludesEquals()
        {
            int[] e1 = new int[0];
            int[] e2 = new int[1] {7};
            int[] e3 = new int[1] {7};

            var x1 = ObjectExtensions.Equals(e3, e2, null);
            var x2 = ObjectExtensions.Equals(e1, e2, null);
            if (x1 != true || x2 != false)
                throw new ApplicationException("Test Failed. Case 0");

            var x3 = ObjectExtensions.Equals(e3.ToList(), e2.ToList(), null);
            var x4 = ObjectExtensions.Equals(e1.ToList(), e2.ToList(), null);
            if (x3 != true || x4 != false)
                throw new ApplicationException("Test Failed. Case 1");

            int[] e4 = new int[1];
            ObjectExtensions.Copy(e2, e4, null);
            if (e4[0]!=e2[0])
                throw new ApplicationException("Test Failed. Case 2");

            try
            {
                ObjectExtensions.Copy(e2, e1, null);
            }
            catch (InvalidOperationException)
            {
                
            }

            var items = new List<Item>() { null, null};
            items.Add(new Item() { F1 = "F1", F2 = "F2", Items = items });
            ObjectExtensions.DetachAll<List<Item>, Item>(items, (i)=>i.Include(e=>e.Items));
        }

        [TestMethod]
        public void IncludesChildNodeTest1()
        {
            var root = new ChainNode(typeof(Point));
            var child = new ChainMemberNode(
                     typeof(int),
                     expression: typeof(Point).CreatePropertyLambda("X"),
                     memberName: "X", isEnumerable: false, parent: root
            );
            root.Children.Add("X", child);
            var include = ChainNodeExtensions.ComposeInclude<Point>(root);
            var p = new Point() { X=1, Y=-1};
            var foramtter = JsonManager.ComposeFormatter<Point>(include);
            var json = foramtter(p);
            if (json != "{\"X\":1}")
                throw new Exception();
        }

        [TestMethod]
        public void IncludesChildNodeTest2()
        {
            var root = new ChainNode(typeof(Point));
            var child = root.AddChild("X");
            var include = ChainNodeExtensions.ComposeInclude<Point>(root);
            var p = new Point() { X = 1, Y = -1 };
            var foramtter = JsonManager.ComposeFormatter<Point>(include);
            var json = foramtter(p);
            if (json != "{\"X\":1}")
                throw new Exception();
        }

        [TestMethod]
        public void IncludesChildNodeTest1Field()
        {
            var root = new ChainNode(typeof(StrangePoint));
            var child = new ChainMemberNode(
                     typeof(int),
                     expression: typeof(StrangePoint).CreateFieldLambda("X"),
                     memberName: "X", isEnumerable: false, parent: root
            );
            root.Children.Add("X", child);
            var include = ChainNodeExtensions.ComposeInclude<StrangePoint>(root);
            var p = new StrangePoint() { X = 1, Y = -1 };
            var foramtter = JsonManager.ComposeFormatter<StrangePoint>(include);
            var json = foramtter(p);
            if (json != "{\"X\":1}")
                throw new Exception();
        }

        [TestMethod]
        public void IncludesChildNodeTest2Field()
        {
            var root = new ChainNode(typeof(StrangePoint));
            var child = root.AddChild("X");
            var child2 = root.AddChild("Point");
            var child3 = child2.AddChild("Y");
            var include = ChainNodeExtensions.ComposeInclude<StrangePoint>(root);
            var xpath = IncludeExtensions.ListLeafXPaths(include);
            // ------------
            var parser = new ChainVisitor<StrangePoint>();
            var includable = new Chain<StrangePoint>(parser);
            if (include != null)
                include.Invoke(includable);
            var node = parser.Root;
            var c = node.Children.Count();
            if (c != 2)
                throw new Exception("<>2");
            // ------------
            var p = new StrangePoint() { X = 1, Y = -1, Point =new Point() { X=10, Y=-10} };
            var foramtter = JsonManager.ComposeFormatter<StrangePoint>(include);
            var json = foramtter(p);
            if (json != "{\"X\":1,\"Point\":{\"Y\":-10}}")
                throw new Exception();
        }

        [TestMethod]
        public void IncludesTestMethod1()
        {
            Include<StrangePoint> include
                = chain => chain
                    .Include(i => i.X)
                    .Include(i => i.GetPoint())
                        .ThenInclude(i => i.Y);
            var xpath = IncludeExtensions.ListLeafXPaths(include);
            // ------------
            var parser = new ChainVisitor<StrangePoint>();
            var includable = new Chain<StrangePoint>(parser);
            if (include != null)
                include.Invoke(includable);
            var node = parser.Root;
            var c = node.Children.Count();
            if (c != 2)
                throw new Exception("<>2");
            var p = new StrangePoint() { X = 1, Y = -1, Point = new Point() { X = 10, Y = -10 } };
            var foramtter = JsonManager.ComposeFormatter<StrangePoint>(include);
            var json = foramtter(p);
            if (json != "{\"X\":1,\"GetPoint\":{\"Y\":-10}}")
                throw new Exception();
        }

        [TestMethod]
        public void IncludesTestMethod2()
        {
            Include<StrangePoint> include
                = chain => chain
                    .Include(i => i.X)
                    .Include(i => i.GetPoint().X+1,"GX");
            var xpath = IncludeExtensions.ListLeafXPaths(include);
            // ------------
            var parser = new ChainVisitor<StrangePoint>();
            var includable = new Chain<StrangePoint>(parser);
            if (include != null)
                include.Invoke(includable);
            var node = parser.Root;
            var c = node.Children.Count();
            if (c != 2)
                throw new Exception("<>2");
            var p = new StrangePoint() { X = 1, Y = -1, Point = new Point() { X = 10, Y = -10 } };
            var foramtter = JsonManager.ComposeFormatter<StrangePoint>(include);
            var json = foramtter(p);
            if (json != "{\"X\":1,\"GX\":11}")
                throw new Exception();
        }

        [TestMethod]
        public void IncludesTestMethodAnd()
        {
            Include<StrangePoint> include
                = chain => chain
                    .Include(i => i.StrangePoint1)
                        .ThenIncluding(i =>i.Point1)
                        .ThenInclude(i => i.Point2)
                    .Include(i => i.StrangePoint2)
                        .ThenIncluding(i => i.Point1)
                        .ThenInclude(i => i.Point2);

            var xpath = IncludeExtensions.ListLeafXPaths(include);
            if (xpath.Count != 4)
                throw new Exception("bad");
        }
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public struct StrangePoint
    {
        public int X;
        public int Y;
        public Point Point;
        public Point GetPoint() { return Point; }
        public StrangePointF StrangePoint1;
        public StrangePointF StrangePoint2;
    }

    public struct StrangePointF
    {
        public Point Point1;
        public Point Point2;
    }
}
