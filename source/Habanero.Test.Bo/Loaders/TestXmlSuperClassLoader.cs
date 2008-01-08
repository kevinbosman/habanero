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

using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Test.BO.Loaders
{
    /// <summary>
    /// Still to test:
    ///   - is ID required for ClassTableInheritance, and what are the implications?
    /// </summary>
    [TestFixture]
    public class TestXmlSuperClassLoader
    {
        private XmlSuperClassLoader itsLoader;

        [SetUp]
        public void SetupTest()
        {
            itsLoader = new XmlSuperClassLoader();
            ClassDef.ClassDefs.Clear();
            ClassDef.LoadClassDefs(
                new XmlClassDefsLoader(
                    @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
						</class>
					</classes>",
                    new DtdLoader()));
        }

        [Test]
        public void TestSimpleProperty()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" />");
            Assert.AreEqual(ORMapping.ClassTableInheritance, def.ORMapping);
            //ClassDef parentDef = ClassDef.ClassDefs[typeof(TestClass)];
            ClassDef parentDef = ClassDef.ClassDefs["Habanero.Test.BO.Loaders", "TestClass"];
            ClassDef superClassDef = def.SuperClassClassDef;
            Assert.AreSame(parentDef, superClassDef);
            Assert.IsNull(def.Discriminator);
        }

        [Test]
        public void TestSingleTableInheritance()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" orMapping=""SingleTableInheritance"" discriminator=""propname"" />");
            Assert.AreEqual(ORMapping.SingleTableInheritance, def.ORMapping);
            Assert.AreEqual("propname", def.Discriminator);
            Assert.IsNull(def.ID);
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestSingleTableInheritanceException()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" orMapping=""SingleTableInheritance"" />");
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestSingleTableInheritanceWithIDException()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" orMapping=""SingleTableInheritance"" id="""" />");
        }

        [Test]
        public void TestConcreteTableInheritance()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" orMapping=""ConcreteTableInheritance"" />");
            Assert.AreEqual(ORMapping.ConcreteTableInheritance, def.ORMapping);
            Assert.IsNull(def.ID);
            Assert.IsNull(def.Discriminator);
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestConcreteTableInheritanceWithIDException()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" orMapping=""ConcreteTableInheritance"" id=""prop"" />");
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestConcreteTableInheritanceWithDiscriminatorException()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" orMapping=""ConcreteTableInheritance"" discriminator="""" />");
        }

        [Test]
        public void TestClassTableInheritanceWithEmptyID()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" id="""" />");
            Assert.AreEqual(ORMapping.ClassTableInheritance, def.ORMapping);
            Assert.AreEqual("", def.ID);
            Assert.IsNull(def.Discriminator);
        }

        [Test]
        public void TestClassTableInheritanceWithID()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" id=""propname"" />");
            Assert.AreEqual(ORMapping.ClassTableInheritance, def.ORMapping);
            Assert.AreEqual("propname", def.ID);
            Assert.IsNull(def.Discriminator);
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestClassTableInheritanceWithDiscriminatorException()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" discriminator="""" />");
        }
    }
}