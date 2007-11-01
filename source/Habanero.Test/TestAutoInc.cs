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

using System;
using System.Collections.Generic;
using System.Text;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;

namespace Habanero.Test
{
    public class TestAutoInc : BusinessObject
    {
        public int? TestAutoIncID
        {
            get
            {
                return (int?) GetPropertyValue("testautoincid");
            }
        }
        public string TestField
        {
            get
            {
                return GetPropertyValueString("testfield");
            }
        }

        public static ClassDef LoadClassDefWithAutoIncrementingID()
        {
            XmlClassLoader itsLoader = new XmlClassLoader();
            ClassDef itsClassDef =
                itsLoader.LoadClass(
                    @"
				<class name=""TestAutoInc"" assembly=""Habanero.Test"" table=""testautoinc"" >
					<property  name=""testautoincid"" type=""Int32"" autoIncrementing=""true"" />
					<property  name=""testfield"" />
					<primaryKey isObjectID=""false"">
						<prop name=""testautoincid"" />
					</primaryKey>
				</class>
			");
            ClassDef.ClassDefs.Add(itsClassDef);
            return itsClassDef;
        }
    }
}