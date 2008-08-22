using System;
using System.Collections.Generic;
using System.Text;
using Habanero.Base;
using Habanero.Base.Exceptions;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestSource_JoinList
    {
        [Test]
        public void TestConstructor()
        {
            //---------------Set up test pack-------------------
            Source source = new Source("TestSource");
            //---------------Execute Test ----------------------
            Source.JoinList joinList = new Source.JoinList(source);
            //---------------Test Result -----------------------
            Assert.AreSame(source, joinList.FromSource);
        }

        [Test]
        public void TestConstructor_NullFromSource()
        {
            //---------------Set up test pack-------------------
            const Source source = null;
            //---------------Execute Test ----------------------
            Exception exception = null;
            try
            {
                Source.JoinList joinList = new Source.JoinList(source);
            } catch (Exception ex)
            {
                exception = ex;
            }
            
            //---------------Test Result -----------------------
            Assert.IsNotNull(exception, "Expected a constructor with null parameter to throw an exception");
            Assert.IsInstanceOfType(typeof(ArgumentNullException), exception);
            ArgumentNullException argumentNullException = (ArgumentNullException)exception;
            Assert.AreEqual("fromSource", argumentNullException.ParamName);
        }

        [Test]
        public void TestAddNewJoinTo()
        {
            //---------------Set up test pack-------------------
            Source source = new Source("TestSource");
            Source.JoinList joinList = new Source.JoinList(source);
            Source toSource = new Source("TestToSource");

            //---------------Execute Test ----------------------
            Source.Join join = joinList.AddNewJoinTo(toSource);

            //---------------Test Result -----------------------
            Assert.IsNotNull(join);
            Assert.AreEqual(1, joinList.Count);
            Assert.AreSame(join, joinList[0]);
        }

        [Test]
        public void TestAddNewJoinTo_AlreadyJoined()
        {
            //---------------Set up test pack-------------------
            Source fromSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList joinList = new Source.JoinList(fromSource);
            Source toSource = new Source("ToSource", "ToSourceEntity");
            Source toSource2 = new Source("ToSource", "ToSourceEntity");
            joinList.AddNewJoinTo(toSource);

            //-------------Assert Preconditions -------------
            Assert.AreEqual(1, joinList.Count);
            
            //---------------Execute Test ----------------------
            joinList.AddNewJoinTo(toSource2);

            //---------------Test Result -----------------------
            Assert.AreEqual(1, joinList.Count);
            Assert.AreSame(fromSource, joinList[0].FromSource);
            Assert.AreSame(toSource, joinList[0].ToSource);
        }

        [Test]
        public void TestAddNewJoinTo_NullSource()
        {
            //---------------Set up test pack-------------------
            Source fromSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList joinList = new Source.JoinList(fromSource);

            //-------------Assert Preconditions -------------
            Assert.AreEqual(0, joinList.Count);

            //---------------Execute Test ----------------------
            joinList.AddNewJoinTo(null);

            //---------------Test Result -----------------------
            Assert.AreEqual(0, joinList.Count);

        }

        [Test]
        public void TestMergeWith()
        {
            //-------------Setup Test Pack ------------------
            Source originalSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList originalJoinList = new Source.JoinList(originalSource);

            Source otherSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList joinList = new Source.JoinList(otherSource);
            Source childSource = new Source("ToSource", "ToSourceEntity");
            Source.Join join = joinList.AddNewJoinTo(childSource);

            QueryField field1 = new QueryField("FromSourceProp1", "FromSourceProp1Field", otherSource);
            QueryField field2 = new QueryField("ToSourceProp1", "ToSourceProp1Field", childSource);
            join.JoinFields.Add(new Source.Join.JoinField(field1, field2));

            //-------------Execute test ---------------------
            originalJoinList.MergeWith(joinList);

            //-------------Test Result ----------------------
            Assert.AreEqual(1, originalJoinList.Count);
            Assert.AreEqual(1, originalJoinList[0].JoinFields.Count);
            Assert.AreEqual(field1, originalJoinList[0].JoinFields[0].FromField);
            Assert.AreEqual(field2, originalJoinList[0].JoinFields[0].ToField);
        }

        [Test]
        public void TestMergeWith_Impossible()
        {
            //-------------Setup Test Pack ------------------
            Source originalSource = new Source("FromSource", "FromSourceEntity");
            Source toSource = new Source("ToSource", "ToSourceEntity");

            //-------------Execute test ---------------------
            Exception exception = null;
            try
            {
                originalSource.MergeWith(toSource);
            }
            catch (Exception ex) { exception = ex; }
            //-------------Test Result ----------------------
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(typeof(HabaneroDeveloperException), exception);
            StringAssert.Contains("A source cannot merge with another source if they do not have the same base source.", exception.Message);
        }

        [Test]
        public void TestMergeWith_EmptySource()
        {
            //-------------Setup Test Pack ------------------
            Source originalSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList originalJoinList = new Source.JoinList(originalSource);
            Source otherSource = new Source("", "FromSourceEntity");
            Source.JoinList joinList = new Source.JoinList(otherSource);

            otherSource.JoinToSource(new Source("ToSource", "ToSourceEntity"));
            //-------------Execute test ---------------------
            originalSource.MergeWith(otherSource);
            //-------------Test Result ----------------------
            Assert.AreEqual(0, originalSource.Joins.Count);
        }

        [Test]
        public void TestMergeWith_NullSource()
        {
            //-------------Setup Test Pack ------------------
            Source fromSource = new Source("FromSource", "FromSourceEntity");

            //-------------Execute test ---------------------
            fromSource.MergeWith(null);
            //-------------Test Result ----------------------
            Assert.AreEqual(0, fromSource.Joins.Count);
        }

        [Test]
        public void TestMergeWith_Simple()
        {
            //-------------Setup Test Pack ------------------
            Source originalSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList originalJoinList = new Source.JoinList(originalSource);
            Source otherSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList joinList = new Source.JoinList(otherSource);
            joinList.AddNewJoinTo(new Source("ToSource", "ToSourceEntity"));

            //-------------Execute test ---------------------
            originalJoinList.MergeWith(joinList);

            //-------------Test Result ----------------------
            Assert.AreEqual(1, originalJoinList.Count);
            Assert.AreSame(joinList[0].ToSource, originalJoinList[0].ToSource);
        }

        [Test]
        public void TestMergeWith_TwoLevels()
        {
            //-------------Setup Test Pack ------------------

            Source originalSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList originalJoinList = originalSource.Joins;
            Source otherSource = new Source("FromSource", "FromSourceEntity");
            Source.JoinList joinList = otherSource.Joins;
            Source childSource = new Source("ChildSource", "ChildSourceEntity");
            Source grandchildSource = new Source("GrandChildSource", "GrandchildSourceEntity");
            otherSource.JoinToSource(childSource);
            childSource.JoinToSource(grandchildSource);
            //-------------Assert Preconditions -------------
            Assert.IsNull(originalSource.ChildSource);
            Assert.IsNotNull(otherSource.ChildSource);
            Assert.IsNotNull(otherSource.ChildSource.ChildSource);
            //-------------Execute test ---------------------
            originalJoinList.MergeWith(joinList);

            //-------------Test Result ----------------------
            Assert.AreEqual(1, originalJoinList.Count);
            Source originalJoinListNewChild = originalJoinList[0].ToSource;
            Assert.AreSame(childSource, originalJoinListNewChild);
            Assert.AreSame(grandchildSource, originalSource.ChildSource.ChildSource);
        }

        //[Test]
        //public void TestMergeWith_IncludesJoinFields()
        //{
        //    //-------------Setup Test Pack ------------------
        //    Source originalSource = new Source("FromSource", "FromSourceEntity");
        //    Source.JoinList originalJoinList = originalSource.Joins;
        //    Source otherSource = new Source("FromSource", "FromSourceEntity");
        //    Source.JoinList joinList = otherSource.Joins;
            
        //    Source childSource = new Source("ToSource", "ToSourceEntity");
        //    otherSource.JoinToSource(childSource);
        //    QueryField field1 = new QueryField("Prop1", "Prop1Field", otherSource);
        //    QueryField field2 = new QueryField("Prop1", "Prop1Field", childSource);
        //    otherSource.Joins[0].JoinFields.Add(new Source.Join.JoinField(field1, field2));

        //    //-------------Execute test ---------------------
        //    originalJoinList.MergeWith(joinList);

        //    //-------------Test Result ----------------------
        //    Assert.AreEqual(1, originalSource.Joins.Count);
        //    Assert.AreEqual(1, originalSource.Joins[0].JoinFields.Count);
        //    Assert.AreEqual(field1, originalSource.Joins[0].JoinFields[0].FromField);
        //    Assert.AreEqual(field2, originalSource.Joins[0].JoinFields[0].ToField);
        //}

        //[Test]
        //public void TestMergeWith_EqualSource()
        //{
        //    //-------------Setup Test Pack ------------------
        //    Source originalSource = new Source("FromSource", "FromSourceEntity");
        //    originalSource.JoinToSource(new Source("ToSource", "ToSourceEntity"));
        //    Source otherSource = new Source("FromSource", "FromSourceEntity");
        //    otherSource.JoinToSource(new Source("ToSource", "ToSourceEntity"));
        //    //-------------Test Pre-conditions --------------

        //    //-------------Execute test ---------------------
        //    originalSource.MergeWith(otherSource);
        //    //-------------Test Result ----------------------
        //    Assert.AreEqual(1, originalSource.Joins.Count);
        //}

        //[Test]
        //public void TestMergeWith_EqualSource_DifferentChildSources()
        //{
        //    //-------------Setup Test Pack ------------------
        //    Source originalSource = new Source("FromSource", "FromSourceEntity");
        //    originalSource.JoinToSource(new Source("ToSource", "ToSourceEntity"));
        //    Source otherSource = new Source("FromSource", "FromSourceEntity");
        //    Source childSource = new Source("ToSource", "ToSourceEntity");
        //    otherSource.JoinToSource(childSource);
        //    childSource.JoinToSource(new Source("GrandchildSource", "GrandchildSourceEntity"));
        //    //-------------Test Pre-conditions --------------

        //    //-------------Execute test ---------------------
        //    originalSource.MergeWith(otherSource);
        //    //-------------Test Result ----------------------
        //    Assert.AreEqual(1, originalSource.Joins.Count);
        //    Assert.AreEqual(1, originalSource.ChildSource.Joins.Count);
        //}
    }
}
