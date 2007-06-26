using Habanero.Bo.ClassDefinition;
using Habanero.Bo.Loaders;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Test.Bo.Loaders
{
    /// <summary>
    /// Summary description for TestXmlSuperClassDescLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlSuperClassDescLoader
    {
        private XmlSuperClassDescLoader itsLoader;

        [SetUp]
        public void SetupTest()
        {
            itsLoader = new XmlSuperClassDescLoader();
            ClassDef.GetClassDefCol.Clear();
            ClassDef.LoadClassDefs(
                new XmlClassDefsLoader(
                    @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.Bo.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKeyDef>
                                <prop name=""TestClassID""/>
                            </primaryKeyDef>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.Bo.Loaders"" >
							<property  name=""TestRelatedClassID"" />
                            <primaryKeyDef>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKeyDef>
						</class>
					</classes>",
                    new DtdLoader()));
        }

        [Test]
        public void TestSimpleProperty()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""Habanero.Test.Bo.Loaders.TestClass"" assembly=""Habanero.Test.Bo"" />");
            Assert.AreEqual(ORMapping.ClassTableInheritance, def.ORMapping);
            Assert.AreSame(ClassDef.GetClassDefCol[typeof (TestClass)], def.SuperClassClassDef);
        }

        [Test]
        public void TestORMapping()
        {
            SuperClassDef def =
                itsLoader.LoadSuperClassDesc(
                    @"<superClass class=""TestClass"" assembly=""Habanero.Test.Bo.Loaders"" orMapping=""SingleTableInheritance"" />");
            Assert.AreEqual(ORMapping.SingleTableInheritance, def.ORMapping);
        }
    }
}