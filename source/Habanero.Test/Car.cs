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

// Static Model
using System;
using System.Collections;
using System.Collections.Generic;
using Habanero.BO.ClassDefinition;
using Habanero.BO;
using Habanero.DB;

namespace Habanero.Test
{
    public class Car : BusinessObject
    {
        #region Constructors

        public Car() : base()
        {
        }

        internal Car(BOPrimaryKey id) : base(id)
        {
        }

        public Car(ClassDef classDef) : base(classDef)
        {
        }

        protected static ClassDef GetClassDef()
        {
            if (!ClassDef.IsDefined(typeof (Car)))
            {
                return CreateClassDef();
            }
            else
            {
                return ClassDef.ClassDefs[typeof (Car)];
            }
        }

        protected override ClassDef ConstructClassDef()
        {
            return GetClassDef();
        }

        private static ClassDef CreateClassDef()
        {
            PropDefCol lPropDefCol = CreateBOPropDef();

            KeyDefCol keysCol = new KeyDefCol();

            PrimaryKeyDef primaryKey = new PrimaryKeyDef();
            primaryKey.IsObjectID = true;
            primaryKey.Add(lPropDefCol["CarID"]);

            RelationshipDefCol relDefCol = CreateRelationshipDefCol(lPropDefCol);


            ClassDef lClassDef = new ClassDef(typeof (Car), primaryKey, lPropDefCol, keysCol, relDefCol);
            ClassDef.ClassDefs.Add(lClassDef);
            return lClassDef;
        }

        private static RelationshipDefCol CreateRelationshipDefCol(PropDefCol lPropDefCol)
        {
            RelationshipDefCol relDefCol = new RelationshipDefCol();

            //Define Owner Relationships
            RelKeyDef relKeyDef = new RelKeyDef();
            PropDef propDef = lPropDefCol["OwnerId"];

            RelPropDef lRelPropDef = new RelPropDef(propDef, "ContactPersonID");
            relKeyDef.Add(lRelPropDef);

            RelationshipDef relDef = new SingleRelationshipDef("Owner", typeof (ContactPerson), relKeyDef, false);

            relDefCol.Add(relDef);

            //Define Driver Relationships
            relKeyDef = new RelKeyDef();
            propDef = lPropDefCol["DriverFK1"];

            lRelPropDef = new RelPropDef(propDef, "PK1Prop1");
            relKeyDef.Add(lRelPropDef);

            propDef = lPropDefCol["DriverFK2"];

            lRelPropDef = new RelPropDef(propDef, "PK1Prop2");
            relKeyDef.Add(lRelPropDef);

            relDef = new SingleRelationshipDef("Driver", typeof (ContactPersonCompositeKey), relKeyDef, true);


            relDefCol.Add(relDef);

            //Define Engine Relationships
            relKeyDef = new RelKeyDef();
            propDef = lPropDefCol["CarID"];

            lRelPropDef = new RelPropDef(propDef, "CarID");
            relKeyDef.Add(lRelPropDef);

            relDef = new SingleRelationshipDef("Engine", typeof (Engine), relKeyDef, false);

            relDefCol.Add(relDef);
            return relDefCol;
        }

        private static PropDefCol CreateBOPropDef()
        {
            PropDefCol lPropDefCol = new PropDefCol();
            PropDef propDef =
                new PropDef("CarRegNo", typeof (String), PropReadWriteRule.ReadWrite, "CAR_REG_NO", null);
            lPropDefCol.Add(propDef);

            lPropDefCol.Add("OwnerId", typeof (Guid), PropReadWriteRule.WriteOnce, "OWNER_ID", null);

            propDef = lPropDefCol.Add("CarID", typeof (Guid), PropReadWriteRule.WriteOnce, "CAR_ID", null);
            propDef =
                lPropDefCol.Add("DriverFK1", typeof (String), PropReadWriteRule.WriteOnce, "Driver_FK1", null);
            propDef =
                lPropDefCol.Add("DriverFK2", typeof (String), PropReadWriteRule.WriteOnce, "Driver_FK2", null);

            return lPropDefCol;
        }

        /// <summary>
        /// returns the Car identified by id.
        /// </summary>
        /// <remarks>
        /// If the Car is already leaded then an identical copy of it will be returned.
        /// </remarks>
        /// <param name="id">The object primary Key</param>
        /// <returns>The loaded business object</returns>
        /// <exception cref="Habanero.BO.BusObjDeleteConcurrencyControlException">
        ///  if the object has been deleted already</exception>
        public static Car GetCar(BOPrimaryKey id)
        {
            Car myCar = (Car)BOLoader.Instance.GetLoadedBusinessObject(id);
            if (myCar == null)
            {
                myCar = new Car(id);
                // AddToLoadedBusinessObjectCol(myCar);
            }
            return myCar;
        }

        #endregion //Constructors

        #region persistance

        #endregion /persistance

        #region Properties

        #endregion //Properties

        #region Relationships

        public ContactPerson GetOwner()
        {
            return (ContactPerson) Relationships.GetRelatedObject("Owner");
        }

        public ContactPersonCompositeKey GetDriver()
        {
            return (ContactPersonCompositeKey) Relationships.GetRelatedObject("Driver");
        }

        public Engine GetEngine()
        {
            return (Engine) Relationships.GetRelatedObject("Engine");
        }

        #endregion //Relationships

        #region ForTesting

        public static void DeleteAllCars()
        {
            string sql = "DELETE FROM Car";
            DatabaseConnection.CurrentConnection.ExecuteRawSql(sql);
        }

        #endregion

        #region ForCollections 

        //class
        protected internal string GetObjectNewID()
        {
            return _primaryKey.GetObjectNewID();
        }

        protected internal static BusinessObjectCollection<BusinessObject> LoadBusinessObjCol()
        {
            return LoadBusinessObjCol("", "");
        }

        protected internal static BusinessObjectCollection<BusinessObject> LoadBusinessObjCol(string searchCriteria,
                                                                                              string orderByClause)
        {
            BusinessObjectCollection<BusinessObject> bOCol = new BusinessObjectCollection<BusinessObject>(GetClassDef());
            bOCol.Load(searchCriteria, orderByClause);
            return bOCol;
        }

        #endregion
    }
}