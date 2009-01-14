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

using System;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.DB;
using Habanero.Test.BO.ClassDefinition;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Test.BO.TransactionCommitters
{
    [TestFixture]
    public class TestTransactionCommitterDB : TestUsingDatabase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
            ClassDef.ClassDefs.Clear();
            CleanStubDatabaseTransactionTable();
            ContactPersonTestBO.DeleteAllContactPeople();
            BORegistry.DataAccessor = new DataAccessorDB();
            BusinessObjectManager.Instance.ClearLoadedObjects();
            
        }

        [TearDown]
        public void TearDownTest()
        {
            //runs every time any testmethod is complete
        }

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            base.SetupDBConnection();
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }

        private static void DoTestCheckForDuplicateObjects()
        {
            ContactPersonTestBO contactPersonCompositeKey = GetSavedContactPersonCompositeKey();
            BusinessObjectManager.Instance.ClearLoadedObjects();
            ContactPersonTestBO duplicateContactPerson = new ContactPersonTestBO
                 {
                     ContactPersonID = Guid.NewGuid(),
                     Surname = contactPersonCompositeKey.Surname,
                     FirstName = contactPersonCompositeKey.FirstName
                 };
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(duplicateContactPerson);
            //---------------Execute Test ----------------------
            try
            {
                committer.CommitTransaction();
                Assert.Fail();
            }
                //---------------Test Result -----------------------
            catch (BusObjDuplicateConcurrencyControlException ex)
            {
                StringAssert.Contains("Surname", ex.Message);
                StringAssert.Contains("FirstName", ex.Message);
            }
            finally
            {
                //---------------Tear Down--------------------------
                contactPersonCompositeKey.MarkForDelete();
                contactPersonCompositeKey.Save();
                if (!duplicateContactPerson.Status.IsNew)
                {
                    duplicateContactPerson.MarkForDelete();
                    duplicateContactPerson.Save();
                }
            }
        }

        private static void AssertTransactionsInTableAre(int expected)
        {
            SqlStatement statement =
                new SqlStatement(DatabaseConnection.CurrentConnection, "select * from stubdatabasetransaction");
            Assert.AreEqual(expected, DatabaseConnection.CurrentConnection.LoadDataTable(statement, "", "").Rows.Count);
        }


        private static void AssertBusinessObjectNotInDatabase(IBusinessObject bo)
        {
//            Criteria criteria = new Criteria(bo.ID[0], );
            try
            {
                BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject(bo.ClassDef, bo.ID);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                StringAssert.Contains("A Error has occured since the object you are trying to refresh has been ", ex.Message);
            }
        }

        private static void AssertBusinessObjectInDatabase(IBusinessObject bo)
        {
//            IBusinessObject loadedBO =
//                BOLoader.Instance.GetBusinessObject(bo.GetType(), bo.ID.ToString());
            IBusinessObject loadedBO =
                BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject(bo.ClassDef, bo.ID);
            Assert.IsNotNull(loadedBO);
        }

        private static void AssertMockBONotInDatabase(Guid mockBOID)
        {
            //    MockBO savedMockBO =
            //        BOLoader.Instance.GetBusinessObject<MockBO>("MockBOID = '" + mockBOID.ToString("B") + "'");
            Criteria criteria = new Criteria("MockBOID", Criteria.ComparisonOp.Equals, mockBOID);
            IBusinessObject savedMockBO =
                BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject<MockBO>((criteria));
            Assert.IsNull(savedMockBO);
        }

        private static void CleanStubDatabaseTransactionTable()
        {
            DatabaseConnection.CurrentConnection.ExecuteRawSql("delete from stubdatabasetransaction");
        }

        private static MockBO CreateSavedMockBO()
        {
            MockBO mockBo = new MockBO();
            mockBo.Save();
            return mockBo;
        }


        private static void CleanupObjectFromDatabase(IBusinessObject bo)
        {
            bo.Delete();
            bo.Save();
        }

        private static ContactPersonTestBO GetSavedContactPersonCompositeKey()
        {
            ContactPersonTestBO contactPersonCompositeKey = GetUnsavedContactPersonCompositeKey();
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(contactPersonCompositeKey);
            committer.CommitTransaction();
            return contactPersonCompositeKey;
        }

        private static ContactPersonTestBO GetUnsavedContactPersonCompositeKey()
        {
            ContactPersonTestBO contactPersonCompositeKey = new ContactPersonTestBO();
            contactPersonCompositeKey.ContactPersonID = Guid.NewGuid();
            contactPersonCompositeKey.Surname = "Somebody";
            contactPersonCompositeKey.FirstName = "Else";
            return contactPersonCompositeKey;
        }

        private class MockBoWithRollBack : MockBO
        {
            private bool _rollBackExecuted;

            public bool RollBackExecuted
            {
                get { return _rollBackExecuted; }
            }

            protected internal override void UpdateAsTransactionRolledBack()
            {
                base.UpdateAsTransactionRolledBack();
                _rollBackExecuted = true;
            }
        }

        [Test]
        public void Test3LayerDeleteRelated()
        {
            //---------------Set up test pack-------------------
            BusinessObjectManager.Instance.ClearLoadedObjects();
            ContactPersonTestBO.DeleteAllContactPeople();
            BORegistry.DataAccessor = new DataAccessorDB();
            OrganisationTestBO.LoadDefaultClassDef();
            TestUtil.WaitForGC();
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_CascadeDelete(out address);

            OrganisationTestBO org = new OrganisationTestBO();
            contactPersonTestBO.SetPropertyValue("OrganisationID", org.OrganisationID);
            org.Save();
            contactPersonTestBO.Save();

            //---------------Execute Test ----------------------
            //IBusinessObjectCollection colContactPeople = org.Relationships["ContactPeople"].GetRelatedBusinessObjectCol();
            //ContactPersonTestBO loadedCP = (ContactPersonTestBO)colContactPeople[0];
            //IBusinessObjectCollection colAddresses = loadedCP.Relationships["Addresses"].GetRelatedBusinessObjectCol();
            //Address loadedAdddress = (Address)colAddresses[0];

            org.MarkForDelete();

            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(org);
            committer.CommitTransaction();

            //---------------Test Result -----------------------
            //Assert.AreSame(contactPersonTestBO, loadedCP);
            //Assert.AreSame(address, loadedAdddress);

            AssertBusinessObjectNotInDatabase(org);
            AssertBusinessObjectNotInDatabase(contactPersonTestBO);
            AssertBusinessObjectNotInDatabase(address);

            BOTestUtils.AssertBOStateIsValidAfterDelete(org);
            BOTestUtils.AssertBOStateIsValidAfterDelete(contactPersonTestBO);
            BOTestUtils.AssertBOStateIsValidAfterDelete(address);
        }

        [Test]
        public void Test3LayerDeleteRelated_WithDeletedObjectChildAndGrandchild()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_CascadeDelete(out address);
            OrganisationTestBO.LoadDefaultClassDef_WithRelationShipToAddress();

            OrganisationTestBO org = new OrganisationTestBO();
            contactPersonTestBO.SetPropertyValue("OrganisationID", org.OrganisationID);
            org.Save();
            contactPersonTestBO.Save();
            address.OrganisationID = org.OrganisationID;
            address.Save();
            org.MarkForDelete();

            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(org);
            //---------------Execute Test ----------------------

            committer.CommitTransaction();
            //---------------Test Result -----------------------
            BOTestUtils.AssertBOStateIsValidAfterDelete(org);
            BOTestUtils.AssertBOStateIsValidAfterDelete(contactPersonTestBO);
            BOTestUtils.AssertBOStateIsValidAfterDelete(address);

            AssertBusinessObjectNotInDatabase(org);
            AssertBusinessObjectNotInDatabase(contactPersonTestBO);
            AssertBusinessObjectNotInDatabase(address);
        }

        [Test]
        public void TestChangeCompositePrimaryKey()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadClassDefWithCompositePrimaryKeyNameSurname();
            ContactPersonTestBO contactPersonCompositeKey = GetSavedContactPersonCompositeKey();
            string oldID = contactPersonCompositeKey.ID.AsString_CurrentValue();
            Assert.IsNotNull(BusinessObjectManager.Instance[oldID]);
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(contactPersonCompositeKey);
            contactPersonCompositeKey.FirstName = "newName";
            //---------------Execute Test ----------------------
            committer.CommitTransaction();
            //---------------Test Result -----------------------
            TransactionCommitterTestHelper.AssertBOStateIsValidAfterInsert_Updated(contactPersonCompositeKey);
            Assert.IsFalse(BusinessObjectManager.Instance.Contains(oldID));
            Assert.IsNotNull(BusinessObjectManager.Instance[contactPersonCompositeKey.ID.AsString_CurrentValue()]);
            //---------------Tear Down--------------------------
            contactPersonCompositeKey.MarkForDelete();
            contactPersonCompositeKey.Save();
        }

        [Test]
        public void TestCheckForDuplicateAlternateKey()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadClassDefWithCompositeAlternateKey();
            DoTestCheckForDuplicateObjects();
        }

        [Test]
        public void TestCheckForDuplicatePrimaryKeys()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadClassDefWithCompositePrimaryKeyNameSurname();
            DoTestCheckForDuplicateObjects();
        }

        [Test]
        public void TestCompositePrimaryUpdatesOrigObjectIDAfterSaving()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadClassDefWithCompositePrimaryKeyNameSurname();
            ContactPersonTestBO contactPersonCompositeKey = new ContactPersonTestBO();
            string oldID = contactPersonCompositeKey.ID.AsString_CurrentValue();
            contactPersonCompositeKey.ContactPersonID = Guid.NewGuid();
            contactPersonCompositeKey.Surname = "Somebody";
            contactPersonCompositeKey.FirstName = "Else";
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(contactPersonCompositeKey);
            //---------------Execute Test ----------------------
            committer.CommitTransaction();
            //---------------Test Result -----------------------
            IPrimaryKey objectID = contactPersonCompositeKey.ID;
            Assert.AreNotEqual(oldID, objectID.AsString_CurrentValue());
            Assert.IsFalse(BusinessObjectManager.Instance.Contains(oldID));
        }

        [Test]
        public void TestDatabaseTransaction_Failure()
        {
            //---------------Set up test pack-------------------
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            StubDatabaseTransaction transactional1 = new StubDatabaseTransaction();
            StubDatabaseFailureTransaction transactional2 = new StubDatabaseFailureTransaction();
            committerDB.AddTransaction(transactional1);
            committerDB.AddTransaction(transactional2);

            //---------------Execute Test ----------------------
            try
            {
                committerDB.CommitTransaction();
            }
            catch (NotImplementedException)
            {
            }
            //---------------Test Result -----------------------
            Assert.IsFalse(transactional1.Committed);
            Assert.IsFalse(transactional2.Committed);
            AssertTransactionsInTableAre(0);
        }

        [Test]
        public void TestDatabaseTransaction_Success()
        {
            //---------------Set up test pack-------------------
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            StubDatabaseTransaction transactional1 = new StubDatabaseTransaction();
            committerDB.AddTransaction(transactional1);

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            Assert.IsTrue(transactional1.Committed);
            AssertTransactionsInTableAre(1);
        }


        [Test]
        public void TestDatabaseTransaction_SuccessTwoTransactions()
        {
            //---------------Set up test pack-------------------
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            StubDatabaseTransaction transactional1 = new StubDatabaseTransaction();
            StubDatabaseTransaction transactional2 = new StubDatabaseTransaction();
            committerDB.AddTransaction(transactional1);
            committerDB.AddTransaction(transactional2);

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            AssertTransactionsInTableAre(2);
        }

        [Test]
        public void TestDeleteDoNothing()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_DeleteDoNothing(out address);
            contactPersonTestBO.MarkForDelete();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(contactPersonTestBO);

            //---------------Execute Test ----------------------
            try
            {
                committerDB.CommitTransaction();
                Assert.Fail("Delete should fail as there is referential integrity and the delete action is 'Do Nothing'");
            }
            catch (DatabaseWriteException ex)
            {
                StringAssert.Contains("There was an error writing to the database", ex.Message);
            }
            //---------------Test Result -----------------------
        }

        [Test]
        public void TestDeleteRelated()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_CascadeDelete(out address);
            contactPersonTestBO.MarkForDelete();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(contactPersonTestBO);

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            BOTestUtils.AssertBOStateIsValidAfterDelete(contactPersonTestBO);
            BOTestUtils.AssertBOStateIsValidAfterDelete(address);

            AssertBusinessObjectNotInDatabase(contactPersonTestBO);
            AssertBusinessObjectNotInDatabase(address);
        }

        [Test,
         Ignore(
             "This test is being ignored due to the fact that we do not have a philosophy for compositional parents deleting their children etc"
             )]
        public void TestDeleteRelatedWithFailure_CancelEditsOnParent()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_CascadeDelete(out address);
            contactPersonTestBO.MarkForDelete();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(contactPersonTestBO);
            committerDB.AddTransaction(new StubDatabaseFailureTransaction());

            try
            {
                committerDB.CommitTransaction();
                Assert.Fail();
            }
            catch (NotImplementedException)
            {
            }

            //---------------Execute Test ----------------------
            contactPersonTestBO.CancelEdits();

            //---------------Test Result -----------------------
            Assert.IsFalse(contactPersonTestBO.Status.IsEditing);
            Assert.IsFalse(contactPersonTestBO.Status.IsDeleted);

            Assert.Ignore(
                "This test is being ignored due to the fact that we do not have a philosophy for compositional parents deleting their children etc");
            Assert.IsFalse(address.Status.IsEditing);
            Assert.IsFalse(address.Status.IsDeleted);
        }

        [Test]
        public void TestDeleteRelatedWithFailureAfter()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_CascadeDelete(out address);
            contactPersonTestBO.MarkForDelete();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(contactPersonTestBO);
            committerDB.AddTransaction(new StubDatabaseFailureTransaction());

            //---------------Execute Test ----------------------
            try
            {
                committerDB.CommitTransaction();
                Assert.Fail();
            }
            catch (NotImplementedException)
            {
            }
            //---------------Test Result -----------------------
            AssertBusinessObjectInDatabase(contactPersonTestBO);
            AssertBusinessObjectInDatabase(address);

            Assert.IsTrue(address.Status.IsDeleted);
            Assert.IsTrue(address.Status.IsEditing);
            Assert.IsFalse(address.Status.IsNew);
            Assert.IsTrue(contactPersonTestBO.Status.IsDeleted);
            Assert.IsTrue(contactPersonTestBO.Status.IsEditing);
            Assert.IsFalse(contactPersonTestBO.Status.IsNew);
        }

        [Test]
        public void TestDereferenceRelatedObjects()
        {
            //The Car has a single relationship to engine. The car->engine relationship is marked 
            // as a dereference related relationship.
            Car.DeleteAllCars();
            Engine.DeleteAllEngines();
            //---------------Set up test pack-------------------

            Car car = new Car();
            car.SetPropertyValue("CarRegNo", "NP32459");
            car.Save();

            Engine engine = new Engine();

            engine.SetPropertyValue("EngineNo", "NO111");
            const string carIDProp = "CarID";
            engine.SetPropertyValue(carIDProp, car.GetPropertyValue(carIDProp));
            engine.Save();
            //Verify test pack - i.e. that engine saved correctly
//            BOLoader.Instance.Refresh(engine);
            BORegistry.DataAccessor.BusinessObjectLoader.Refresh(engine);
            Assert.AreSame(engine.GetCar(), car);

            //---------------Execute Test ----------------------
            car.MarkForDelete();
            car.Save();

            //---------------Test Result -----------------------
            Assert.IsNull(engine.GetPropertyValue(carIDProp));
            Assert.IsNull(engine.GetCar());
            //---------------Test TearDown -----------------------

            Car.DeleteAllCars();
            Engine.DeleteAllEngines();
        }

        [Test]
        public void TestMultipleStatementsInOneITransactional()
        {
            //---------------Set up test pack-------------------
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            StubDatabaseTransactionMultiple transactional1 = new StubDatabaseTransactionMultiple();
            committerDB.AddTransaction(transactional1);

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            AssertTransactionsInTableAre(2);
        }

        [Test]
        public void TestNotPreventDelete_ForNonDeleted()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_PreventDelete(out address);
            contactPersonTestBO.FirstName = Guid.NewGuid().ToString();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(contactPersonTestBO);
            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();
            //---------------Test Result -----------------------
            Assert.IsFalse(contactPersonTestBO.Status.IsDirty);
        }

        [Test]
        public void TestPersistSimpleBO_Delete()
        {
            //---------------Set up test pack-------------------
            MockBO mockBo = CreateSavedMockBO();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            mockBo.MarkForDelete();
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(mockBo));

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            BOTestUtils.AssertBOStateIsValidAfterDelete(mockBo);
            AssertMockBONotInDatabase(mockBo.MockBOID);
        }

        [Test]
        public void TestPersistSimpleBO_Delete_InvalidData()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadClassDefWithAddressesRelationship_DeleteRelated();
            ContactPersonTestBO contactPersonTestBO = ContactPersonTestBO.CreateSavedContactPersonNoAddresses();
            contactPersonTestBO.Surname = null;
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(contactPersonTestBO));

            //---------------Execute Test ----------------------
            contactPersonTestBO.MarkForDelete();
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            BOTestUtils.AssertBOStateIsValidAfterDelete(contactPersonTestBO);
        }

        [Test]
        public void TestPersistSimpleBO_Insert()
        {
            //---------------Set up test pack-------------------
            MockBO mockBo = new MockBO();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(mockBo));

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            TransactionCommitterTestHelper.AssertBOStateIsValidAfterInsert_Updated(mockBo);
//            BOLoader.Instance.Refresh(mockBo);
            BORegistry.DataAccessor.BusinessObjectLoader.Refresh(mockBo);
            //MockBO savedMockBO =
            //    BOLoader.Instance.GetBusinessObject<MockBO>("MockBOID = '" + mockBo.MockBOID.ToString("B") + "'");
            Criteria criteria = new Criteria("MockBOID", Criteria.ComparisonOp.Equals, mockBo.MockBOID);
            MockBO savedMockBO = BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject<MockBO>(criteria);
            Assert.AreSame(mockBo, savedMockBO);
        }

        [Test, ExpectedException(typeof (BusObjectInAnInvalidStateException))]
        public void TestPersistSimpleBO_Insert_InvalidData()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadDefaultClassDef();
            ContactPersonTestBO contactPersonTestBO = new ContactPersonTestBO();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(contactPersonTestBO));

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();
            //TODO Brett 12 Jan 2009: Validate on save 
            //---------------Test Result -----------------------
            //AssertBOStateIsValidAfterInsert_Updated(contactPersonTestBO);
            //BOLoader.Instance.Refresh(contactPersonTestBO);
            //MockBO savedMockBO =
            //    BOLoader.Instance.GetBusinessObject<MockBO>("MockBOID = '" + contactPersonTestBO.MockBOID.ToString("B") + "'");
            //Assert.AreSame(contactPersonTestBO, savedMockBO);
        }

        [Test]
        public void TestPersistSimpleBO_NewDelete()
        {
            //---------------Set up test pack-------------------
            MockBO mockBo = new MockBO();
            mockBo.SetStatus(BOStatus.Statuses.isNew, false);
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            mockBo.MarkForDelete();
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(mockBo));

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            BOTestUtils.AssertBOStateIsValidAfterDelete(mockBo);
            AssertMockBONotInDatabase(mockBo.MockBOID);
        }

        [Test]
        public void TestPersistSimpleBO_Update()
        {
            //---------------Set up test pack-------------------
            MockBO mockBo = CreateSavedMockBO();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            Guid mockBOProp1 = Guid.NewGuid();
            mockBo.MockBOProp1 = mockBOProp1;
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(mockBo));

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            TransactionCommitterTestHelper.AssertBOStateIsValidAfterInsert_Updated(mockBo);
            BORegistry.DataAccessor.BusinessObjectLoader.Refresh(mockBo);
            Assert.AreEqual(mockBOProp1, mockBo.MockBOProp1);
        }

        [Test]
        public void TestPersistSimpleBO_Update_NotUsingTransactionalBusinessObject_DB()
        {
            //---------------Set up test pack-------------------
            MockBO mockBo = CreateSavedMockBO();
            Guid mockBOProp1 = Guid.NewGuid();
            mockBo.MockBOProp1 = mockBOProp1;
            TransactionCommitter committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(mockBo);

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            TransactionCommitterTestHelper.AssertBOStateIsValidAfterInsert_Updated(mockBo);
            BORegistry.DataAccessor.BusinessObjectLoader.Refresh(mockBo);
            Assert.AreEqual(mockBOProp1, mockBo.MockBOProp1);
        }

        [Test]
        public void TestPersistSimpleBO_UpdateNonDirty()
        {
            //---------------Set up test pack-------------------
            MockBO mockBo = CreateSavedMockBO();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            Guid? mockBOProp1 = mockBo.MockBOProp1;
            committerDB.AddTransaction(new TransactionalBusinessObjectDB(mockBo));

            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();

            //---------------Test Result -----------------------
            TransactionCommitterTestHelper.AssertBOStateIsValidAfterInsert_Updated(mockBo);
            BORegistry.DataAccessor.BusinessObjectLoader.Refresh(mockBo);
            Assert.AreEqual(mockBOProp1, mockBo.MockBOProp1);
        }

        [Test, ExpectedException(typeof (BusinessObjectReferentialIntegrityException))]
        public void TestPreventDelete()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_PreventDelete(out address);
            contactPersonTestBO.MarkForDelete();
            TransactionCommitterDB committerDB = new TransactionCommitterDB();
            committerDB.AddBusinessObject(contactPersonTestBO);
            //---------------Execute Test ----------------------
            committerDB.CommitTransaction();
            //---------------Test Result -----------------------
        }

        [Test]
        public void TestPreventDelete_ThreeLevels()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_PreventDelete(out address);
            OrganisationTestBO.LoadDefaultClassDef();

            OrganisationTestBO org = new OrganisationTestBO();
            contactPersonTestBO.SetPropertyValue("OrganisationID", org.OrganisationID);
            org.Save();
            contactPersonTestBO.Save();
            org.MarkForDelete();

            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(org);
            //---------------Execute Test ----------------------

            try
            {
                committer.CommitTransaction();
                Assert.Fail();
            }
                //---------------Test Result -----------------------
            catch (BusinessObjectReferentialIntegrityException ex)
            {
                Assert.IsTrue(
                    ex.Message.Contains("There are 1 objects related through the 'ContactPeople.Addresses' relationship"));
            }
            finally
            {
                CleanupObjectFromDatabase(address);
                CleanupObjectFromDatabase(contactPersonTestBO);
                CleanupObjectFromDatabase(org);
            }
        }

        [Test]
        public void TestRollBack_OnError()
        {
            //---------------Set up test pack-------------------
            //Create object in DB
            MockBoWithRollBack mockBo = new MockBoWithRollBack();
            StubDatabaseFailureTransaction trnFail = new StubDatabaseFailureTransaction();
            TransactionCommitterStubDB trnCommitter = new TransactionCommitterStubDB();
            trnCommitter.AddBusinessObject(mockBo);
            trnCommitter.AddTransaction(trnFail);
            Assert.IsFalse(mockBo.RollBackExecuted);
            //---------------Execute Test ----------------------
            try
            {
                trnCommitter.CommitTransaction();
                Assert.Fail();
            }
                //---------------Test Result -----------------------
                //Raise Exception that the object has been edited since 
                // the user last edited.
            catch (NotImplementedException)
            {
                Assert.IsTrue(mockBo.RollBackExecuted);
            }
        }

        [Test]
        public void TestSearchForChildrenToDeleteWithChildDeletedInTransaction()
        {
            //---------------Set up test pack-------------------
            AddressTestBO address;
            ContactPersonTestBO contactPersonTestBO =
                ContactPersonTestBO.CreateContactPersonWithOneAddress_CascadeDelete(out address);

            //---------------Execute Test ----------------------
            address.MarkForDelete();
            contactPersonTestBO.MarkForDelete();
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(address);
            committer.AddBusinessObject(contactPersonTestBO);
            committer.CommitTransaction();
            //---------------Test Result -----------------------

            BOTestUtils.AssertBOStateIsValidAfterDelete(address);
            BOTestUtils.AssertBOStateIsValidAfterDelete(contactPersonTestBO);
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestStorePersistedValue_BOProp()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadDefaultClassDef();
            ContactPersonTestBO cp = new ContactPersonTestBO();
            cp.Surname = Guid.NewGuid().ToString();
            cp.Save();
            cp.Surname = Guid.NewGuid().ToString();
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(cp);

            //---------------Execute Test ----------------------
            committer.CommitTransaction();
            //---------------Test Result -----------------------
            Assert.AreEqual(cp.Surname, cp.Props["Surname"].PersistedPropertyValueString);
        }


        /// <summary>
        /// the <see cref="TransactionCommitterDB"/>shouldn't check for duplicates on the primary key as the object is new
        /// and has an autoincrementing id field.
        /// </summary>
        [Test]
        public void TestAutoIncrementingFieldInNewPrimaryKeyDoesntCheckDuplicates()
        {
            //---------------Set up test pack-------------------
            ClassDef.ClassDefs.Clear();
            TestAutoInc.LoadClassDefWithAutoIncrementingID();

            TestAutoInc bo = new TestAutoInc();
            bo.SetPropertyValue("testfield", "testing 123");

            MockRepository mockRepos = new MockRepository();
            IBusinessObjectLoader mockBusinessObjectLoader = mockRepos.DynamicMock<IBusinessObjectLoader>();

            Expect.Call(
                mockBusinessObjectLoader.GetBusinessObjectCollection(null, new Criteria("", Criteria.ComparisonOp.Equals, "")))
                .IgnoreArguments()
                .Repeat.Never();

            DataAccessorStub dataAccessor = new DataAccessorStub();
            dataAccessor.BusinessObjectLoader = mockBusinessObjectLoader;

            BORegistry.DataAccessor = dataAccessor;
            mockRepos.ReplayAll();
            TransactionCommitterDB transactionCommitterDB = new TransactionCommitterDB();
            transactionCommitterDB.AddBusinessObject(bo);

            //---------------Execute Test ----------------------
            transactionCommitterDB.CommitTransaction();

            //---------------Test Result -----------------------
            mockRepos.VerifyAll();

        }
        private class DataAccessorStub : IDataAccessor
        {
            private IBusinessObjectLoader _businessObjectLoader;
            public IBusinessObjectLoader BusinessObjectLoader { get { return _businessObjectLoader; } set { _businessObjectLoader = value; } }

            public ITransactionCommitter CreateTransactionCommitter() { return null; }
        }
    }
}