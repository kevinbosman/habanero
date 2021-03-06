#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System;
using System.Globalization;
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Test.Structure;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestReflectionPropertyMapper
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            ClassDef.ClassDefs.Clear();
            GlobalRegistry.UIExceptionNotifier = new RethrowingExceptionNotifier();
            BORegistry.DataAccessor = new DataAccessorInMemory();
            BORegistry.BusinessObjectManager = new BusinessObjectManagerNull();
            AddressTestBO.LoadDefaultClassDef();
            ContactPersonTestBO.LoadClassDefOrganisationTestBORelationship_MultipleReverse();
            OrganisationTestBO.LoadDefaultClassDef();
        }
        [SetUp]
        public void SetupTest()
        {

        }

        [Test]
        public void Test_Construct()
        {
            //---------------Set up test pack-------------------
            string propertyName = TestUtil.GetRandomString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ReflectionPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propertyName);
            //---------------Test Result -----------------------
            Assert.AreEqual(propertyName, boPropertyMapper.PropertyName);
            Assert.IsNull(boPropertyMapper.BusinessObject);;
        }

        [Test]
        public void Test_Construct_WhenNullPropertyName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new ReflectionPropertyMapper(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propertyName", ex.ParamName);
            }
        }

        [Test]
        public void Test_Construct_WhenStringEmptyPropertyName_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                new ReflectionPropertyMapper("");
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propertyName", ex.ParamName);
            }
        }

        [Test]
        public void Test_BusinessObject_GetAndSet()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            const string propertyName = "FirstName";
            ReflectionPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propertyName);
            //---------------Assert Precondition----------------
            Assert.IsNull(boPropertyMapper.BusinessObject);
            //---------------Execute Test ----------------------
            boPropertyMapper.BusinessObject = contactPersonTestBO;
            //---------------Test Result -----------------------
            Assert.AreSame(contactPersonTestBO, boPropertyMapper.BusinessObject);
        }

        [Test]
        public void Test_BusinessObject_WhenSetWithBONotHavingSpecifiedProperty_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            const string propertyName = "SomeNonExistentProperty";
            ReflectionPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propertyName);
            //---------------Assert Precondition----------------
            Assert.IsNull(boPropertyMapper.BusinessObject);
            //---------------Execute Test ----------------------
            try
            {
                boPropertyMapper.BusinessObject = contactPersonTestBO;
                Assert.Fail("Expected to throw a HabaneroDeveloperException");
            }
            //---------------Test Result -----------------------
            catch (InvalidPropertyException ex)
            {
                StringAssert.Contains("The property '" + propertyName + "' on '"
                                      + contactPersonTestBO.ClassDef.ClassName + "' cannot be found. Please contact your system administrator.", ex.Message);
                Assert.IsNull(boPropertyMapper.BusinessObject);
            }
        }

        [Test]
        public void Test_SetPropertyValue_ShouldSetBOPropsValue()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            const string propName = "ReflectiveProp";
            IBOPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propName) { BusinessObject = contactPersonTestBO };
            //---------------Assert Precondition----------------
            Assert.IsNull(contactPersonTestBO.ReflectiveProp);
            //---------------Execute Test ----------------------
            var expectedPropValue = RandomValueGen.GetRandomString();
            boPropertyMapper.SetPropertyValue(expectedPropValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropValue, contactPersonTestBO.ReflectiveProp);
        }
        [Test]
        public void Test_SetPropertyValue_WhenBONull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            const string propName = "ReflectiveProp";
            ReflectionPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propName);
            //---------------Assert Precondition----------------
            Assert.IsNull(boPropertyMapper.BusinessObject);
            //---------------Execute Test ----------------------
            try
            {
                boPropertyMapper.SetPropertyValue(RandomValueGen.GetRandomString());
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                string expectedErrorMessage = string.Format(
                    "Tried to Set Property Value the ReflectionPropertyMapper for Property '{0}' when the BusinessObject is not set "
                    , propName);
                StringAssert.Contains(expectedErrorMessage, ex.Message);
            }
        }

        [Test]
        public void Test_GetPropertyValue_ShouldSetBOPropsValue()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            const string propName = "ReflectiveProp";
            IBOPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propName) { BusinessObject = contactPersonTestBO };
            var expectedPropValue = RandomValueGen.GetRandomString();
            contactPersonTestBO.ReflectiveProp = expectedPropValue;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(contactPersonTestBO.ReflectiveProp);
            Assert.AreEqual(expectedPropValue, contactPersonTestBO.ReflectiveProp);
            //---------------Execute Test ----------------------

            object actualValue = boPropertyMapper.GetPropertyValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedPropValue, actualValue);
            Assert.AreEqual(expectedPropValue, contactPersonTestBO.ReflectiveProp);
        }

        [Test]
        public void Test_GetPropertyValue_WhenBONull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            const string propName = "ReflectiveProp";
            ReflectionPropertyMapper boPropertyMapper = new ReflectionPropertyMapper(propName);
            //---------------Assert Precondition----------------
            Assert.IsNull(boPropertyMapper.BusinessObject);
            //---------------Execute Test ----------------------
            try
            {
                boPropertyMapper.GetPropertyValue();
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                string expectedErrorMessage = string.Format(
                    "Tried to GetPropertyValue the ReflectionPropertyMapper for Property '{0}' when the BusinessObject is not set "
                    , propName);
                StringAssert.Contains(expectedErrorMessage, ex.Message);
            }
        }
        [Test]
        public void Test_InvalidMessage_WhenPropNull_ShouldReturnStdMessage()
        {
            //---------------Set up test pack-------------------
            ReflectionPropertyMapperStub propMapper = new ReflectionPropertyMapperStub();
            PropertyInfo boPropStub = null;
            propMapper.SetPropInfo(boPropStub);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var invalidMessage = propMapper.InvalidReason;
            //---------------Test Result -----------------------
            var expectedMessage = string.Format("The Property '{0}' is not available"
                     , propMapper.PropertyName);
            Assert.AreEqual(expectedMessage, invalidMessage);
        }
        [Test]
        public void Test_InvalidMessage_WhenMessageSet_ShouldReturnMessage()
        {
            //---------------Set up test pack-------------------
            ReflectionPropertyMapperStub propMapper = new ReflectionPropertyMapperStub();
            propMapper.SetPropInfo(new FakePropertyInfo());
            var expectedMessage = RandomValueGen.GetRandomString();
            propMapper.SetIvalidMessage(expectedMessage);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var invalidMessage = propMapper.InvalidReason;
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedMessage, invalidMessage);
        }
        [Test]
        public void Test_InvalidMessage_WhenMessageSet_ThenSetValue_ShouldReturnEmptyMessage()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            const string propName = "ReflectiveProp";
            ReflectionPropertyMapperStub propMapper = new ReflectionPropertyMapperStub(propName) { BusinessObject = contactPersonTestBO };
            var initialMessage = RandomValueGen.GetRandomString();
            propMapper.SetIvalidMessage(initialMessage);
            //---------------Assert Precondition----------------
            Assert.AreEqual(initialMessage, propMapper.InvalidReason);
            //---------------Execute Test ----------------------
            propMapper.SetPropertyValue(RandomValueGen.GetRandomString());
            var invalidMessage = propMapper.InvalidReason;
            //---------------Test Result -----------------------
            Assert.AreEqual("", invalidMessage);
        }

        [Test]
        public void Test_InvalidMessage_WhenMessageSet_ThenSetBO_ShouldReturnEmptyMessage()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            const string propName = "ReflectiveProp";
            ReflectionPropertyMapperStub propMapper = new ReflectionPropertyMapperStub(propName) { BusinessObject = contactPersonTestBO };
            var initialMessage = RandomValueGen.GetRandomString();
            propMapper.SetIvalidMessage(initialMessage);
            //---------------Assert Precondition----------------
            Assert.AreEqual(initialMessage, propMapper.InvalidReason);
            //---------------Execute Test ----------------------
            propMapper.BusinessObject = new ContactPersonTestBO();
            var invalidMessage = ((IBOPropertyMapper)propMapper).InvalidReason;
            //---------------Test Result -----------------------
            Assert.AreEqual("", invalidMessage);
        }
    }

    class ReflectionPropertyMapperStub : ReflectionPropertyMapper
    {
        public ReflectionPropertyMapperStub()
            : base(RandomValueGen.GetRandomString())
        {
        }
        public ReflectionPropertyMapperStub(string propertyName)
            : base(propertyName)
        {
        }


        public void SetIvalidMessage(string message)
        {
            _invalidMessage = message;
        }

        public void SetPropInfo(PropertyInfo prop)
        {
            _propertyInfo = prop;
        }
    }
    public class FakePropertyInfo : PropertyInfo
    {
        private readonly Type _declaringType;
        private readonly string _propName;
        private readonly Type _propType;
        private Type _reflectedType;

        public FakePropertyInfo(string propName, Type propType)
        {
            _propName = propName;
            _propType = propType;
        }
        public FakePropertyInfo(Type declaringType)
        {
            _declaringType = declaringType;
        }
        public FakePropertyInfo()
        {
            _declaringType = MockRepository.GenerateMock<Type>();
            _propType = MockRepository.GenerateMock<Type>();
            _propName = RandomValueGen.GetRandomString();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return null;
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return null;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return _propName; }
        }

        /// <summary>
        /// Gets the class that declares this member.
        /// </summary>
        /// <returns>
        /// The Type object for the class that declares this member.
        /// </returns>
        public override Type DeclaringType
        {
            get { return _declaringType; }
        }

        public override Type ReflectedType
        {
            get { return _reflectedType; }
        }
        public void SetReflectedType(Type reflectedType)
        {
            _reflectedType = reflectedType;
        }

        public override Type PropertyType
        {
            get { return _propType; }
        }

        public override PropertyAttributes Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
}