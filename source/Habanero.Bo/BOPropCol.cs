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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Habanero.BO
{
    /// <summary>
    /// Manages a collection of BOProp objects
    /// </summary>
    public class BOPropCol 
    {
        private Dictionary<string, BOProp> _boProps;
        private string _autoIncrementingPropertyName;

        /// <summary>
        /// Constructor to initialise a new empty collection
        /// </summary>
        internal BOPropCol() : base()
        {
            _boProps = new Dictionary<string, BOProp>();
        }

        /// <summary>
        /// Adds a property to the collection
        /// </summary>
        /// <param name="prop">The property to add</param>
        internal void Add(BOProp prop)
        {
            if (Contains(prop.PropertyName.ToUpper()))
            {
                throw new InvalidPropertyException(String.Format(
                    "The BOProp with the name '{0}' is being added to the " +
                    "prop collection, but already exists in the collection.",
                    prop.PropertyName));
            }
            _boProps.Add(prop.PropertyName.ToUpper(), prop);
        }

        /// <summary>
        /// Copies the properties from another collection into this one
        /// </summary>
        /// <param name="propCol">A collection of properties</param>
        internal void Add(BOPropCol propCol)
        {
            foreach (BOProp prop in propCol.Values)
            {
                this.Add(prop);
            }
        }

        /// <summary>
        /// Remove a specified property from the collection
        /// </summary>
        /// <param name="propName">The property name</param>
        internal void Remove(string propName)
        {
            _boProps.Remove(propName.ToUpper());
        }

        /// <summary>
        /// Indicates whether the collection contains the property specified
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <returns>Returns true if found</returns>
        public bool Contains(string propName)
        {
            return _boProps.ContainsKey(propName.ToUpper());
        }

        /// <summary>
        /// Provides an indexing facility so the contents of the collection
        /// can be accessed with square brackets like an array
        /// </summary>
        /// <param name="propName">The name of the property to access</param>
        /// <returns>Returns the property if found, or null if not</returns>
        public BOProp this[string propName]
        {
            get
            {
                if (!Contains(propName.ToUpper()))
                {
                    throw new InvalidPropertyNameException(String.Format(
                        "A BOProp with the name '{0}' does not exist in the " +
                        "prop collection.", propName));
                }
                return _boProps[propName.ToUpper()];
            }
        }

        /// <summary>
        /// Returns an xml string containing the properties whose values
        /// have changed, along with their old and new values
        /// </summary>
        internal string DirtyXml
        {
            get
            {
                string dirtlyXml = "<Properties>";
                foreach (BOProp prop in this.SortedValues )
                {
                    if (prop.IsDirty)
                    {
                        dirtlyXml += prop.DirtyXml;
                    }
                }
                return dirtlyXml;
            }
        }

        /// <summary>
        /// Restores each of the property values to their PersistedValue
        /// </summary>
        internal void RestorePropertyValues()
        {
            foreach (BOProp prop in this)
            {
                prop.RestorePropValue();
            }
        }

        /// <summary>
        /// Copies across each of the properties' current values to their
        /// persisted values
        /// </summary>
        internal void BackupPropertyValues()
        {
            foreach (BOProp prop in this)
            {
                prop.BackupPropValue();
            }
        }

        /// <summary>
        /// Indicates whether all of the held property values are valid
        /// </summary>
        /// <param name="invalidReason">A string to alter if one or more
        /// property values are invalid</param>
        /// <returns>Returns true if all the property values are valid, false
        /// if any one is invalid</returns>
        internal bool IsValid(out string invalidReason)
        {
            bool propsValid = true;
            StringBuilder reason = new StringBuilder();
            foreach (BOProp prop in this)
            {
                if (!prop.isValid)
                {
                    reason.Append(prop.InvalidReason + Environment.NewLine);
                    propsValid = false;
                }
            }
            invalidReason = reason.ToString();
            return propsValid;
        }

        /// <summary>
        /// Sets the IsObjectNew setting in each property to that specified
        /// </summary>
        /// <param name="bValue">Whether the object is set as new</param>
        internal void setIsObjectNew(bool bValue)
        {
            foreach (BOProp prop in this)
            {
                prop.IsObjectNew = bValue;
            }
        }

        /// <summary>
        /// Returns a collection containing all the values being held
        /// </summary>
        public ICollection  Values
        {
            get { return _boProps.Values; }
        }

        /// <summary>
        /// Returns the collection of property values as a SortedList
        /// </summary>
        public IEnumerable SortedValues
        {
            get { return new SortedList(_boProps).Values; }
        }

        public IEnumerator GetEnumerator()
        {
            return SortedValues.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return _boProps.Count;
            }
        }

        public string AutoIncrementingPropertyName
        {
            get { return _autoIncrementingPropertyName; }
            set { _autoIncrementingPropertyName = value; }
        }
    }
}
