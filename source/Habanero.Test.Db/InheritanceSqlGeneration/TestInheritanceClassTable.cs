//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.DB;
using NUnit.Framework;

namespace Habanero.Test.DB.InheritanceSqlGeneration
{
    /// <summary>
    /// Tests the default type of class table inheritance, where the child contains a
    /// foreign key to the parent ID that is not the child's ID field
    /// </summary>
    [TestFixture]
    public class TestInheritanceClassTable : TestInheritanceBase
    {
        public static void RunTest()
        {
            TestInheritanceClassTable test = new TestInheritanceClassTable();
            test.SetupTest();
            test.TestSuperClassKey();
        }

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            SetupTest();
        }

        protected override void SetupInheritanceSpecifics()
        {
            //Circle.GetClassDef().SuperClassDef =
            //    new SuperClassDef(Shape.GetClassDef(), ORMapping.ClassTableInheritance);
            //Circle.GetClassDef().SuperClassDef.ID = "ShapeID";
            Circle.GetClassDefWithClassTableInheritance();
        }

        protected override void SetStrID()
        {
            strID = (string) DatabaseUtil.PrepareValue(objCircle.GetPropertyValue("CircleID"));
        }

        [Test]
        public void TestCircleIsUsingClassTableInheritance()
        {
            Assert.AreEqual(ORMapping.ClassTableInheritance, Circle.GetClassDef().SuperClassDef.ORMapping);
        }

        [Test]
        public void TestCircleIsNotDirty()
        {
            Circle circle = new Circle();
            Assert.IsFalse(circle.Status.IsDirty);
        }

        [Test]
        public void TestCircleHasCircleIDAsPrimaryKey()
        {
            try
            {
                Assert.IsTrue(objCircle.ID.Contains("CircleID"));
                Assert.AreEqual(1, objCircle.ID.Count,
                                "There should only be one item in the primary key (even when using class table inheritance).");
            }
            catch (HabaneroArgumentException)
            {
                Assert.Fail("An object using ClassTableInheritance should have the subclass' primary key.");
            }
        }

        [Test]
        public void TestCircleHasCorrectPropertyNames()
        {
            objCircle.GetPropertyValue("ShapeName");
            objCircle.GetPropertyValue("Radius");
            objCircle.GetPropertyValue("CircleID");
            objCircle.GetPropertyValue("ShapeID");
        }

        [Test]
        public void TestCircleInsertSql()
        {
            Assert.AreEqual(2, itsInsertSql.Count,
                            "There should be 2 insert sql statements when using class table inheritance");
            Assert.AreEqual("INSERT INTO `Shape_table` (`ShapeID_field`, `ShapeName`) VALUES (?Param0, ?Param1)",
                            itsInsertSql[0].Statement.ToString(),
                            "Class Table inheritance: First insert Sql statement is incorrect.");
            Assert.AreEqual(strID, itsInsertSql[0].Parameters[0].Value,
                            "Parameter ShapeID has incorrect value in first insert statement using class table inheritance");
            Assert.AreEqual("MyShape", itsInsertSql[0].Parameters[1].Value,
                            "Parameter ShapeName has incorrect value in first insert statement using class table inheritance");
            Assert.AreEqual("INSERT INTO `circle_table` (`CircleID_field`, `Radius`, `ShapeID_field`) VALUES (?Param0, ?Param1, ?Param2)",
                            itsInsertSql[1].Statement.ToString(),
                            "Class Table inheritance: Second Sql statement is incorrect.");
            Assert.AreEqual(strID, itsInsertSql[1].Parameters[0].Value,
                            "Parameter CircleID has incorrect value in second insert statement using class table inheritance.");
            Assert.AreEqual(strID, itsInsertSql[1].Parameters[2].Value,
                            "Parameter ShapeID has incorrect value in second insert statement using class table inheritance.");
            Assert.AreEqual(10, itsInsertSql[1].Parameters[1].Value,
                            "Parameter Radius has incorrect value in second insert statement using class table inheritance.");
        }

        [Test]
        public void TestSuperClassKey()
        {
            IBOKey msuperKey = BOPrimaryKey.GetSuperClassKey(Circle.GetClassDef(), objCircle);
            Assert.IsTrue(msuperKey.Contains("ShapeID"), "Super class key should contain the ShapeID property");
            Assert.AreEqual(1, msuperKey.Count, "Super class key should only have one prop");
            Assert.AreEqual(msuperKey["ShapeID"].Value, objCircle.ID["CircleID"].Value,
                            "ShapeID and CircleID should be the same");
        }

        [Test]
        public void TestCircleUpdateSql()
        {
            Assert.AreEqual(2, itsUpdateSql.Count,
                            "There should be 2 update sql statements when using class table inheritance");
            Assert.AreEqual("UPDATE `Shape_table` SET `ShapeID_field` = ?Param0, `ShapeName` = ?Param1 WHERE `ShapeID_field` = ?Param2",
                            itsUpdateSql[0].Statement.ToString(),
                            "Class table inheritance: first update sql statement is incorrect.");
            Assert.AreEqual(strID, itsUpdateSql[0].Parameters[0].Value,
                            "Parameter ShapeID has incorrect value in first update statement using class table inheritance");
            Assert.AreEqual("MyShape", itsUpdateSql[0].Parameters[1].Value,
                            "Parameter ShapeName has incorrect value in first update statement using class table inheritance");
            Assert.AreEqual(strID, itsUpdateSql[0].Parameters[2].Value,
                            "Parameter ShapeID in where clause has incorrect value in first update statement using class table inheritance");
            Assert.AreEqual("UPDATE `circle_table` SET `Radius` = ?Param0 WHERE `CircleID_field` = ?Param1",
                            itsUpdateSql[1].Statement.ToString(),
                            "Class table inheritance: second update sql statement is incorrect.");
            Assert.AreEqual(10, itsUpdateSql[1].Parameters[0].Value,
                            "Parameter Radius has incorrect value in second update statement using class table inheritance");
            Assert.AreEqual(strID, itsUpdateSql[1].Parameters[1].Value,
                            "Parameter CircleID has incorrect value in second update statement using class table inheritance");
        }

        [Test]
        public void TestCircleDeleteSql()
        {
            Assert.AreEqual(2, itsDeleteSql.Count,
                            "There should be 2 delete sql statements when using class table inheritance.");
            Assert.AreEqual("DELETE FROM `circle_table` WHERE `CircleID_field` = ?Param0", itsDeleteSql[0].Statement.ToString(),
                            "Class table inheritance: first delete sql statement is incorrect.");
            Assert.AreEqual(strID, itsDeleteSql[0].Parameters[0].Value,
                            "Parameter CircleID has incorrect value in first delete statement in where clause.");
            Assert.AreEqual("DELETE FROM `Shape_table` WHERE `ShapeID_field` = ?Param0", itsDeleteSql[1].Statement.ToString(),
                            "Class table inheritance: second delete sql statement is incorrect.");
            Assert.AreEqual(strID, itsDeleteSql[1].Parameters[0].Value,
                            "Parameter ShapeID has incorrect value in second delete statement in where clause.");
        }

        [Test]
        public void TestDatabaseReadWrite()
        {
            // Test inserting & selecting

            Shape shape = new Shape();
            shape.ShapeName = "MyShape";
            shape.Save();

            BusinessObjectCollection<Shape> shapes = new BusinessObjectCollection<Shape>();
            shapes.LoadAll();
            Assert.AreEqual(1, shapes.Count);

            BusinessObjectCollection<Circle> circles = new BusinessObjectCollection<Circle>();
            circles.LoadAll();
            Assert.AreEqual(0, circles.Count);

            Circle circle = new Circle();
            circle.Radius = 5;
            circle.ShapeName = "Circle";
            circle.Save();

            shapes.LoadAll("ShapeName");
            Assert.AreEqual(2, shapes.Count);
            Assert.AreEqual("Circle", shapes[0].ShapeName);
            Assert.AreEqual("MyShape", shapes[1].ShapeName);

            circles.LoadAll();
            Assert.AreEqual(1, circles.Count);
            Assert.AreEqual(circles[0].ShapeID, shapes[0].ShapeID);
            Assert.IsNotNull(circles[0].CircleID);
            Assert.AreEqual(5, circles[0].Radius);
            Assert.AreEqual("Circle", circles[0].ShapeName);

            // Test updating
            shape.ShapeName = "MyShapeChanged";
            shape.Save();
            circle.ShapeName = "CircleChanged";
            circle.Radius = 10;
            circle.Save();

            shapes.LoadAll("ShapeName");
            Assert.AreEqual("CircleChanged", shapes[0].ShapeName);
            Assert.AreEqual("MyShapeChanged", shapes[1].ShapeName);
            circles.LoadAll();
            Assert.AreEqual(10, circles[0].Radius);
            Assert.AreEqual("CircleChanged", circles[0].ShapeName);
            
            // Test deleting
            shape.MarkForDelete();
            shape.Save();
            circle.MarkForDelete();
            circle.Save();
            shapes.LoadAll();
            Assert.AreEqual(0, shapes.Count);
            circles.LoadAll();
            Assert.AreEqual(0, circles.Count);
        }

        // Provided in case the above test fails and the rows remain in the database
        [TestFixtureTearDown]
        public void TearDown()
        {
            Criteria shapeCriteria = new Criteria(
                new Criteria("ShapeName", Criteria.ComparisonOp.Equals, "MyShape"), 
                Criteria.LogicalOp.Or,
                new Criteria("ShapeName", Criteria.ComparisonOp.Equals, "MyShapeChanged"));
            Shape shape = BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject<Shape>(shapeCriteria);
            if (shape != null)
            {
                shape.MarkForDelete();
                shape.Save();
            }

            Criteria criteria = new Criteria(
                new Criteria("ShapeName", Criteria.ComparisonOp.Equals, "Circle"),
                Criteria.LogicalOp.Or,
                new Criteria("ShapeName", Criteria.ComparisonOp.Equals, "CircleChanged"));
            Circle circle = BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject<Circle>(criteria);
            if (circle != null)
            {
                circle.MarkForDelete();
                circle.Save();
            }
        }
    }
}