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
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Exceptions;

namespace Habanero.BO
{
    /// <summary>
    /// Defines methods for loading BusinessObjects and BusinessObjectCollections for any datastore e.g. Database 
    /// XML File, CSV.  For any new type of data store (eg file based, or TCP, or
    /// web service based etc), this interface must be implemented. To enable saving to the data store a subclass of  
    /// <see cref="TransactionCommitter"/> must be implemented for your data store type, and class that implements  
    /// <see cref="IDataAccessor"/>  must be created that links these two classes by providing a method of getting the loader 
    /// and a transactioncommitter.  This class is accessed via the BORegistry.
    /// 
    /// To load a collection from scratch you might do this:
    /// <code>
    ///   Criteria surnameCriteria = new Criterea("Surname", Criteria.ComparisonOp.Equals, "Smith");
    ///   BusinessObjectCollection&lt;Person&gt; personCol = 
    ///          BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObjectCollection&lt;Person&gt;(surnameCriteria);
    /// </code>
    /// 
    /// You can pass in your own SelectQuery object if you wish to create a custom select query to load your object or collection.
    /// By default a SelectQuery is built up from the ClassDef loaded for the type, taking into account inheritance structures.
    /// 
    /// When loading one or more object from the datastore the Business Object loader should check to see if it is already loaded in 
    /// the object manager first. If the object does not exist in the object manager then it must be loaded from the datastore and added
    /// to the object manager <see cref="BusinessObjectManager"/>
    /// </summary>
    public interface IBusinessObjectLoader
    {
        /// <summary>
        /// Loads a business object of type T, using the Primary key given as the criteria
        /// </summary>
        /// <typeparam name="T">The type of object to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="primaryKey">The primary key to use to load the business object</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an <see cref="HabaneroDeveloperException"/> error is throw</returns>
        T GetBusinessObject<T>(IPrimaryKey primaryKey) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a business object of the type identified by a <see cref="ClassDef"/>, using the Primary key given as the criteria
        /// </summary>
        /// <param name="classDef">The ClassDef of the object to load.</param>
        /// <param name="primaryKey">The primary key to use to load the business object</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an <see cref="HabaneroDeveloperException"/> error is throw</returns>
        IBusinessObject GetBusinessObject(IClassDef classDef, IPrimaryKey primaryKey);

        /// <summary>
        /// Loads a business object of type T, using the criteria given
        /// </summary>
        /// <typeparam name="T">The type of object to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteria">The criteria to use to load the business object</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an <see cref="HabaneroDeveloperException"/> error is throw</returns>
        T GetBusinessObject<T>(Criteria criteria) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a business object of the type identified by a <see cref="ClassDef"/>, using the criteria given
        /// </summary>
        /// <param name="classDef">The ClassDef of the object to load.</param>
        /// <param name="criteria">The criteria to use to load the business object</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an <see cref="HabaneroDeveloperException"/> error is throw</returns>
        IBusinessObject GetBusinessObject(IClassDef classDef, Criteria criteria);

        /// <summary>
        /// Loads a business object of type T, using the SelectQuery given. It's important to make sure that T (meaning the ClassDef set up for T)
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a business object
        /// </summary>
        /// <typeparam name="T">The type of object to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="selectQuery">The select query to use to load from the data source</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found, an error is raised</returns>
        T GetBusinessObject<T>(ISelectQuery selectQuery) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a business object of the type identified by a <see cref="ClassDef"/>, 
        /// using the SelectQuery given. It's important to make sure that the ClassDef parameter given
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a business object
        /// </summary>
        /// <param name="classDef">The ClassDef of the object to load.</param>
        /// <param name="selectQuery">The select query to use to load from the data source</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an <see cref="HabaneroDeveloperException"/> error is throw</returns>
        IBusinessObject GetBusinessObject(IClassDef classDef, ISelectQuery selectQuery);

        /// <summary>
        /// Loads a business object of type T, using the SelectQuery given. It's important to make sure that T (meaning the ClassDef set up for T)
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a business object
        /// </summary>
        /// <typeparam name="T">The type of object to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteriaString">The select query to use to load from the data source</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an <see cref="HabaneroDeveloperException"/> error is throw</returns>
        T GetBusinessObject<T>(string criteriaString) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a business object of the type identified by a <see cref="ClassDef"/>, using the criteria given
        /// </summary>
        /// <param name="classDef">The ClassDef of the object to load.</param>
        /// <param name="criteriaString">The criteria to use to load the business object must be of formst "PropName = criteriaValue" e.g. "Surname = Powell"</param>
        /// <returns>The business object that was found. If none was found, null is returned. If more than one is found an error is raised</returns>
        IBusinessObject GetBusinessObject(IClassDef classDef, string criteriaString);

        /// <summary>
        /// Loads a business object of type T using the relationship given. The relationship will be converted into a
        /// Criteria object that defines the relationship and this will be used to load the related object.
        /// </summary>
        /// <typeparam name="T">The type of the business object to load</typeparam>
        /// <param name="relationship">The relationship to use to load the object</param>
        /// <returns>An object of type T if one was found, otherwise null</returns>
        T GetRelatedBusinessObject<T>(SingleRelationship<T> relationship) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a business object using the relationship given. The relationship will be converted into a
        /// Criteria object that defines the relationship and this will be used to load the related object.
        /// </summary>
        /// <param name="relationship">The relationship to use to load the object</param>
        /// <returns>An object of the type defined by the relationship if one was found, otherwise null</returns>
        IBusinessObject GetRelatedBusinessObject(ISingleRelationship relationship);

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(Criteria criteria) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteriaString">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(string criteriaString) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given. 
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, Criteria criteria);

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given, applying the order criteria to order the collection that is returned. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        /// <param name="orderCriteria">The order criteria to use (ie what fields to order the collection on)</param>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(Criteria criteria, IOrderCriteria orderCriteria) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given, applying the order criteria to order the collection that is returned. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteriaString">The criteria to use to load the business object collection</param>
        /// <param name="orderCriteria">The order criteria to use (ie what fields to order the collection on)</param>
        /// <returns>The loaded collection</returns>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(string criteriaString, string orderCriteria) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads business objects that match the search criteria provided, 
        /// loaded in the order specified, and limiting the number of objects loaded. 
        /// The limited list of Ts specified as follows:
        /// If you want record 6 to 15 then 
        /// <paramref name="firstRecordToLoad"/> will be set to 5 (this is zero based) and 
        /// <paramref name="numberOfRecordsToLoad"/> will be set to 10.
        /// This will load 10 records, starting at record 6 of the ordered set (ordered by the <paramref name="orderCriteria"/>).
        /// If there are fewer than 15 records in total, then the remaining records after record 6 willbe returned. 
        /// </summary>
        /// <remarks>
        /// As a design decision, we have elected for the <paramref name="firstRecordToLoad"/> to be zero based since this is consistent with the limit clause in used by MySql etc.
        /// Also, the <paramref name="numberOfRecordsToLoad"/> returns the specified number of records unless its value is '-1' where it will 
        /// return all the remaining records from the specified <paramref name="firstRecordToLoad"/>.
        /// If you give '0' as the value for the <paramref name="numberOfRecordsToLoad"/> parameter, it will load zero records.
        /// </remarks>
        /// <example>
        /// The following code demonstrates how to loop through the invoices in the data store, 
        /// ten at a time, and print their details:
        /// <code>
        /// BusinessObjectCollection&lt;Invoice&gt; col = new BusinessObjectCollection&lt;Invoice&gt;();
        /// int interval = 10;
        /// int firstRecord = 0;
        /// int totalNoOfRecords = firstRecord + 1;
        /// while (firstRecord &lt; totalNoOfRecords)
        /// {
        ///     col.LoadWithLimit("", "InvoiceNo", firstRecord, interval, out totalNoOfRecords);
        ///     Debug.Print("The next {0} invoices:", interval);
        ///     col.ForEach(bo =&gt; Debug.Print(bo.ToString()));
        ///     firstRecord += interval;
        /// }</code>
        /// </example>
        /// <param name="criteria">The search criteria</param>
        /// <param name="orderCriteria">The order-by clause</param>
        /// <param name="firstRecordToLoad">The first record to load (NNB: this is zero based)</param>
        /// <param name="numberOfRecordsToLoad">The number of records to be loaded</param>
        /// <param name="totalNoOfRecords">The total number of records matching the criteria</param>
        /// <returns>The loaded collection, limited in the specified way.</returns>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(Criteria criteria, IOrderCriteria orderCriteria,
                int firstRecordToLoad, int numberOfRecordsToLoad, out int totalNoOfRecords)
            where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads business objects that match the search criteria provided, 
        /// loaded in the order specified, and limiting the number of objects loaded. 
        /// The limited list of Ts specified as follows:
        /// If you want record 6 to 15 then 
        /// <paramref name="firstRecordToLoad"/> will be set to 5 (this is zero based) and 
        /// <paramref name="numberOfRecordsToLoad"/> will be set to 10.
        /// This will load 10 records, starting at record 6 of the ordered set (ordered by the <paramref name="orderCriteriaString"/>).
        /// If there are fewer than 15 records in total, then the remaining records after record 6 willbe returned. 
        /// </summary>
        /// <remarks>
        /// As a design decision, we have elected for the <paramref name="firstRecordToLoad"/> to be zero based since this is consistent with the limit clause in used by MySql etc.
        /// Also, the <paramref name="numberOfRecordsToLoad"/> returns the specified number of records unless its value is '-1' where it will 
        /// return all the remaining records from the specified <paramref name="firstRecordToLoad"/>.
        /// If you give '0' as the value for the <paramref name="numberOfRecordsToLoad"/> parameter, it will load zero records.
        /// </remarks>
        /// <example>
        /// The following code demonstrates how to loop through the invoices in the data store, 
        /// ten at a time, and print their details:
        /// <code>
        /// BusinessObjectCollection&lt;Invoice&gt; col = new BusinessObjectCollection&lt;Invoice&gt;();
        /// int interval = 10;
        /// int firstRecord = 0;
        /// int totalNoOfRecords = firstRecord + 1;
        /// while (firstRecord &lt; totalNoOfRecords)
        /// {
        ///     col.LoadWithLimit("", "InvoiceNo", firstRecord, interval, out totalNoOfRecords);
        ///     Debug.Print("The next {0} invoices:", interval);
        ///     col.ForEach(bo =&gt; Debug.Print(bo.ToString()));
        ///     firstRecord += interval;
        /// }</code>
        /// </example>
        /// <param name="criteriaString">The search criteria</param>
        /// <param name="orderCriteriaString">The order-by clause</param>
        /// <param name="firstRecordToLoad">The first record to load (NNB: this is zero based)</param>
        /// <param name="numberOfRecordsToLoad">The number of records to be loaded</param>
        /// <param name="totalNoOfRecords">The total number of records matching the criteria</param>
        /// <returns>The loaded collection, limited in the specified way.</returns>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(string criteriaString, string orderCriteriaString,
                int firstRecordToLoad, int numberOfRecordsToLoad, out int totalNoOfRecords)
            where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads the Business object collection with the appropriate items.
        /// See <see cref="GetBusinessObjectCollection{T}(Criteria,OrderCriteria,int,int,out int)"/> for a full explanation.
        /// </summary>
        /// <param name="def"></param>
        /// <param name="criteria"></param>
        /// <param name="orderCriteria"></param>
        /// <param name="firstRecordToLoad"></param>
        /// <param name="numberOfRecordsToLoad"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        IBusinessObjectCollection GetBusinessObjectCollection(IClassDef def, Criteria criteria,
                IOrderCriteria orderCriteria, int firstRecordToLoad, int numberOfRecordsToLoad, out int records);

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given, applying the order criteria to order the collection that is returned. 
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        /// <param name="orderCriteria">The order criteria to use (ie what fields to order the collection on)</param>
        IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, Criteria criteria, IOrderCriteria orderCriteria);

        /// <summary>
        /// Loads a BusinessObjectCollection using the SelectQuery given. It's important to make sure that T (meaning the ClassDef set up for T)
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a businessobjectcollection so that you can perhaps load from multiple
        /// tables using a join (if loading from a database source).
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="selectQuery">The select query to use to load from the data source</param>
        /// <returns>The loaded collection</returns>
        BusinessObjectCollection<T> GetBusinessObjectCollection<T>(ISelectQuery selectQuery) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a BusinessObjectCollection using the SelectQuery given. It's important to make sure that the ClassDef given
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a businessobjectcollection so that you can perhaps load from multiple
        /// tables using a join (if loading from a database source).
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="selectQuery">The select query to use to load from the data source</param>
        /// <returns>The loaded collection</returns>
        IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, ISelectQuery selectQuery);

        /// <summary>
        /// Loads a BusinessObjectCollection using the searchCriteria an given. It's important to make sure that the ClassDef given
        /// has the properties defined in the fields of the select searchCriteria and orderCriteria.  
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="searchCriteria">The select query to use to load from the data source</param>
        /// <param name="orderCriteria">The order that the collections must be loaded in e.g. Surname, FirstName</param>
        /// <returns>The loaded collection</returns>
        IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, string searchCriteria, string orderCriteria);

        /// <summary>
        /// Loads a BusinessObjectCollection using the searchCriteria an given. It's important to make sure that the ClassDef given
        /// has the properties defined in the fields of the select searchCriteria and orderCriteria.  
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="searchCriteria">The select query to use to load from the data source</param>
        /// <returns>The loaded collection</returns>
        IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, string searchCriteria);

        /// <summary>
        /// Reloads a BusinessObjectCollection using the criteria it was originally loaded with.  You can also change the criteria or order
        /// it loads with by editing its SelectQuery object. The collection will be cleared as such and reloaded (although Added events will
        /// only fire for the new objects added to the collection, not for the ones that already existed).
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="collection">The collection to refresh</param>
        void Refresh<T>(BusinessObjectCollection<T> collection) where T : class, IBusinessObject, new();

        /// <summary>
        /// Reloads a BusinessObjectCollection using the criteria it was originally loaded with.  You can also change the criteria or order
        /// it loads with by editing its SelectQuery object. The collection will be cleared as such and reloaded (although Added events will
        /// only fire for the new objects added to the collection, not for the ones that already existed).
        /// </summary>
        /// <param name="collection">The collection to refresh</param>
        void Refresh(IBusinessObjectCollection collection);

        /// <summary>
        /// Reloads a businessObject from the datasource using the id of the object.
        /// A dirty object will not be refreshed from the database and the appropriate error will be raised.
        /// Cancel all edits before refreshing the object or save before refreshing.
        /// </summary>
        /// <exception cref="HabaneroDeveloperException">Exception thrown if the object is dirty and refresh is called.</exception>
        /// <param name="businessObject">The businessObject to refresh</param>
        IBusinessObject Refresh(IBusinessObject businessObject);

        /// <summary>
        /// Loads a RelatedBusinessObjectCollection using the Relationship given.  This method is used by relationships to load based on the
        /// fields defined in the relationship.
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="relationship">The relationship that defines the criteria that must be loaded.  For example, a Person might have
        /// a Relationship called Addresses, which defines the PersonID property as the relationship property. In this case, calling this method
        /// with the Addresses relationship will load a collection of Address where PersonID = '?', where the ? is the value of the owning Person's
        /// PersonID</param>
        /// <returns>The loaded RelatedBusinessObjectCollection</returns>
        RelatedBusinessObjectCollection<T> GetRelatedBusinessObjectCollection<T>(IMultipleRelationship relationship) where T : class, IBusinessObject, new();

        /// <summary>
        /// Loads a RelatedBusinessObjectCollection using the Relationship given.  This method is used by relationships to load based on the
        /// fields defined in the relationship.
        /// </summary>
        /// <param name="type">The type of collection to load. This must be a class that implements IBusinessObject</param>
        /// <param name="relationship">The relationship that defines the criteria that must be loaded.  For example, a Person might have
        /// a Relationship called Addresses, which defines the PersonID property as the relationship property. In this case, calling this method
        /// with the Addresses relationship will load a collection of Address where PersonID = '?', where the ? is the value of the owning Person's
        /// PersonID</param>
        /// <returns>The loaded RelatedBusinessObjectCollection</returns>
        IBusinessObjectCollection GetRelatedBusinessObjectCollection(Type type, IMultipleRelationship relationship);

        ///<summary>
        /// For a given value e.g. a Guid Identifier '{......}' this will 
        /// load the business object from the Data store.
        /// This can only be used for business objects that have a single property for the primary key
        /// (i.e. non composite primary keys)
        ///</summary>
        ///<param name="classDef">The Class definition of the Business Object to load</param>
        ///<param name="idValue">The value of the primary key of the business object</param>
        ///<returns>the Business Object that matches the value of the id. If the primary key cannot be constructed
        /// e.g. the primary key is composite then returns null. If the Business Object cannot be loaded then returns
        /// <see cref="BusObjDeleteConcurrencyControlException"/>
        ///  </returns>
        /// <exception cref="BusObjDeleteConcurrencyControlException"/>
        IBusinessObject GetBusinessObjectByValue(IClassDef classDef, object idValue);

        ///<summary>
        /// For a given value e.g. a Guid Identifier '{......}' this will 
        /// load the business object from the Data store.
        /// This can only be used for business objects that have a single property for the primary key
        /// (i.e. non composite primary keys)
        ///</summary>
        ///<param name="type">The type of business object to be loaded</param>
        ///<param name="idValue">The value of the primary key of the business object</param>
        ///<returns>the Business Object that matches the value of the id. If the primary key cannot be constructed
        /// e.g. the primary key is composite then returns null. If the Business Object cannot be loaded then returns
        /// <see cref="BusObjDeleteConcurrencyControlException"/>
        ///  </returns>
        /// <exception cref="BusObjDeleteConcurrencyControlException"/>
        IBusinessObject GetBusinessObjectByValue(Type type, object idValue);

        ///<summary>
        /// For a given value e.g. a Guid Identifier '{......}' this will 
        /// load the business object from the Data store.
        /// This can only be used for business objects that have a single property for the primary key
        /// (i.e. non composite primary keys)
        ///</summary>
        ///<param name="idValue">The value of the primary key of the business object</param>
        ///<returns>the Business Object that matches the value of the id. If the primary key cannot be constructed
        /// e.g. the primary key is composite then returns null. If the Business Object cannot be loaded then returns
        /// <see cref="BusObjDeleteConcurrencyControlException"/>
        ///  </returns>
        /// <exception cref="BusObjDeleteConcurrencyControlException"/>
        T GetBusinessObjectByValue<T>(object idValue) where T : class, IBusinessObject, new();

        /// <summary>
        /// Reloads a BusinessObjectCollection using the criteria it was originally loaded with.  You can also change the criteria or order
        /// it loads with by editing its SelectQuery object. The collection will be cleared as such and reloaded (although Added events will
        /// only fire for the new objects added to the collection, not for the ones that already existed).
        /// </summary>
        int GetCount(IClassDef classDef, Criteria criteria);

    }
}
