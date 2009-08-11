using System.IO;
using Db4objects.Db4o;
using Habanero.BO;
using Habanero.DB4O;
using Habanero.Test.BO.BusinessObjectCollection;
using NUnit.Framework;

namespace Habanero.Test.DB4O
{
    [TestFixture]
    public class TestRelatedBOCol_Association_WithDB4O : TestRelatedBOCol_Association
    {
        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            if (DB4ORegistry.DB != null) DB4ORegistry.DB.Close();
            const string db4oFileStore = "DataStore.db4o";
            if (File.Exists(db4oFileStore)) File.Delete(db4oFileStore);
            DB4ORegistry.DB = Db4oFactory.OpenFile(db4oFileStore);
            BORegistry.DataAccessor = new DataAccessorDB4O(DB4ORegistry.DB);
        }
    }
}