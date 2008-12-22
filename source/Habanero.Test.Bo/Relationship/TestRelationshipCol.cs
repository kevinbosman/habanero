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

using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    /// <summary>
    /// Summary description for TestRelationshipCol.
    /// </summary>
    [TestFixture]
    public class TestRelationshipCol : TestUsingDatabase
    {
        private ClassDef itsClassDef;
        private ClassDef itsRelatedClassDef;

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            base.SetupDBConnection();
            ClassDef.ClassDefs.Clear();
            itsClassDef = MyBO.LoadClassDefWithRelationship();
            itsRelatedClassDef = MyRelatedBo.LoadClassDef();
        }

        [
            Test,
                ExpectedException(typeof (RelationshipNotFoundException),
                    ExpectedMessage ="The relationship WrongRelationshipName was not found on a BusinessObject of type Habanero.Test.MyBO"
                    )]
        public void TestMissingRelationshipErrorMessageSingle()
        {
            MyBO bo1 = (MyBO) itsClassDef.CreateNewBusinessObject();
            bo1.Relationships.GetRelatedObject<MyBO>("WrongRelationshipName");
        }

        [
            Test,
                ExpectedException(typeof (RelationshipNotFoundException),
                    ExpectedMessage = "The relationship WrongRelationshipName was not found on a BusinessObject of type Habanero.Test.MyBO"
                    )]
        public void TestMissingRelationshipErrorMessageMultiple()
        {
            MyBO bo1 = (MyBO) itsClassDef.CreateNewBusinessObject();
            bo1.Relationships.GetRelatedCollection("WrongRelationshipName");
        }

        [
            Test,
                ExpectedException(typeof (InvalidRelationshipAccessException),
                    ExpectedMessage = "The 'single' relationship MyRelationship was accessed as a 'multiple' relationship (using GetRelatedCollection())."
                    )]
        public void TestInvalidRelationshipAccessSingle()
        {
            MyBO bo1 = (MyBO) itsClassDef.CreateNewBusinessObject();
            bo1.Relationships.GetRelatedCollection("MyRelationship");
        }

        [
            Test,
                ExpectedException(typeof (InvalidRelationshipAccessException),
                    ExpectedMessage="The 'multiple' relationship MyMultipleRelationship was accessed as a 'single' relationship (using GetRelatedObject())."
                    )]
        public void TestInvalidRelationshipAccessMultiple()
        {
            MyBO bo1 = (MyBO) itsClassDef.CreateNewBusinessObject();
            bo1.Relationships.GetRelatedObject<MyBO>("MyMultipleRelationship");
        }

        [Test]
        public void TestSetRelatedBusinessObject()
        {
            MyBO bo1 = (MyBO) itsClassDef.CreateNewBusinessObject();
            MyRelatedBo relatedBo1 = (MyRelatedBo) itsRelatedClassDef.CreateNewBusinessObject();
            bo1.Relationships.SetRelatedObject("MyRelationship", relatedBo1);
            Assert.AreSame(relatedBo1, bo1.Relationships.GetRelatedObject<MyRelatedBo>("MyRelationship"));
            Assert.AreSame(bo1.GetPropertyValue("RelatedID"), relatedBo1.GetPropertyValue("MyRelatedBoID"));
        }

        [
            Test,
                ExpectedException(typeof (InvalidRelationshipAccessException),
                    ExpectedMessage="SetRelatedObject() was passed a relationship (MyMultipleRelationship) that is of type 'multiple' when it expects a 'single' relationship"
                    )]
        public void TestSetRelatedBusinessObjectWithWrongRelationshipType()
        {
            MyBO bo1 = (MyBO) itsClassDef.CreateNewBusinessObject();
            MyRelatedBo relatedBo1 = (MyRelatedBo) itsRelatedClassDef.CreateNewBusinessObject();
            bo1.Relationships.SetRelatedObject("MyMultipleRelationship", relatedBo1);
        }

        //[Test]
        //public void TestCreateBusinessObject()
        //{
        //    ContactPersonTestBO.LoadDefaultClassDef();
        //    ContactPerson person = new ContactPerson();

        //    Address address = person.Addresses.CreateBusinessObject();

        //    Assert.AreSame(person, address.ContactPerson);
        //}
    }
}