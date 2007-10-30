//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
// 
// This file is part of Habanero Standard.
// 
//     Habanero Standard is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Habanero Standard is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with Habanero Standard.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using NUnit.Framework;

namespace Habanero.Test.BO.Loaders
{
    /// <summary>
    /// Summary description for TestXmlUIPropertyLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlUIFormPropertyLoader
    {
        private XmlUIFormFieldLoader loader;

        [SetUp]
        public void SetupTest()
        {
            loader = new XmlUIFormFieldLoader();
        }

        [Test]
        public void TestSimpleUIProperty()
        {
            UIFormField uiProp =
                loader.LoadUIProperty(
                    @"<field label=""testlabel"" property=""testpropname"" type=""Button"" mapperType=""testmappertypename"" mapperAssembly=""testmapperassembly"" editable=""false"" />");
            Assert.AreEqual("testlabel", uiProp.Label);
            Assert.AreEqual("testpropname", uiProp.PropertyName);
            Assert.AreEqual("Button", uiProp.ControlType.Name);
            Assert.AreEqual("testmappertypename", uiProp.MapperTypeName);
            Assert.AreEqual("testmapperassembly", uiProp.MapperAssembly);
            Assert.AreEqual(false, uiProp.Editable);
        }

        [Test]
        public void TestDefaults()
        {
            UIFormField uiProp =
                loader.LoadUIProperty(@"<field label=""testlabel"" property=""testpropname"" />");
            Assert.AreEqual("testlabel", uiProp.Label);
            Assert.AreEqual("testpropname", uiProp.PropertyName);
            Assert.AreEqual("TextBox", uiProp.ControlType.Name);
            Assert.AreEqual("TextBoxMapper", uiProp.MapperTypeName);
            Assert.AreEqual(true, uiProp.Editable);
        }


        [Test]
        public void TestPropertyAttributes()
        {
            UIFormField uiProp =
                loader.LoadUIProperty(
                    @"<field label=""testlabel"" property=""testpropname"" ><parameter name=""TestAtt"" value=""TestValue"" /><parameter name=""TestAtt2"" value=""TestValue2"" /></field>");
            Assert.AreEqual("TestValue", uiProp.GetParameterValue("TestAtt"));
            Assert.AreEqual("TestValue2", uiProp.GetParameterValue("TestAtt2"));
        }

    }
}