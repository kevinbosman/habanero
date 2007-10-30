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
using Habanero.BO;
using Habanero.DB;
using Habanero.Util;

namespace Habanero.BO.ClassDefinition
{
    /// <summary>
    /// Defines a relationship where the owner relates to only one other object.
    /// </summary>
    public class SingleRelationshipDef : RelationshipDef
    {
	
		#region Constructors

        /// <summary>
        /// Constructor to create a new single relationship definition
        /// </summary>
        /// <param name="relationshipName">A name for the relationship</param>
        /// <param name="relatedObjectClassType">The class type of the related object</param>
        /// <param name="relKeyDef">The related key definition</param>
        /// <param name="keepReferenceToRelatedObject">Whether to keep a
        /// reference to the related object.  Could be false for memory-
        /// intensive applications.</param>
        public SingleRelationshipDef(string relationshipName, Type relatedObjectClassType, RelKeyDef relKeyDef,
                                     bool keepReferenceToRelatedObject)
            : base(relationshipName, relatedObjectClassType, relKeyDef, keepReferenceToRelatedObject)
        {
		}

		/// <summary>
		/// Constructor to create a new single relationship definition
		/// </summary>
		/// <param name="relationshipName">A name for the relationship</param>
		/// <param name="relatedObjectAssemblyName">The assembly name of the related object</param>
		/// <param name="relatedObjectClassName">The class name of the related object</param>
		/// <param name="relKeyDef">The related key definition</param>
		/// <param name="keepReferenceToRelatedObject">Whether to keep a
        /// reference to the related object.  Could be false for memory-
        /// intensive applications.</param>
		public SingleRelationshipDef(string relationshipName, string relatedObjectAssemblyName, string relatedObjectClassName, RelKeyDef relKeyDef,
									 bool keepReferenceToRelatedObject)
			: base(relationshipName, relatedObjectAssemblyName, relatedObjectClassName, relKeyDef, keepReferenceToRelatedObject)
		{
		}

		#endregion Constructors

		/// <summary>
        /// Overrides abstract method of RelationshipDef to create a new
        /// relationship
        /// </summary>
        /// <param name="owningBo">The business object that will manage
        /// this relationship</param>
        /// <param name="lBOPropCol">The collection of properties</param>
        /// <returns></returns>
        public override Relationship CreateRelationship(BusinessObject owningBo, BOPropCol lBOPropCol)
        {
            return new SingleRelationship(owningBo, this, lBOPropCol);
        }
    }

}