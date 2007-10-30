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
using System.Xml;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Base;
using Habanero.Util;

namespace Habanero.BO.Loaders
{
    /// <summary>
    /// Uses a variety of xml loaders to load all the facets that make
    /// up a business object class definition
    /// </summary>
    public class XmlClassLoader : XmlLoader
    {
        private PropDefCol _PropDefCol;
        private PrimaryKeyDef _PrimaryKeyDef;
        private KeyDefCol _KeyDefCol;
        private RelationshipDefCol _RelationshipDefCol;
        private string _ClassName;
        private string _AssemblyName;
        private SuperClassDef _SuperClassDef;
        private UIDefCol _UIDefCol;
        private string _TableName;
        //private bool _SupportsSynchronising;

        /// <summary>
        /// Constructor to initialise a new loader
        /// </summary>
        public XmlClassLoader()
        {
        }

        /// <summary>
        /// Constructor to initialise a new loader with a dtd path
        /// </summary>
		/// <param name="dtdLoader">The dtd loader</param>
		/// <param name="defClassFactory">The factory for the definition classes</param>
        public XmlClassLoader(DtdLoader dtdLoader, IDefClassFactory defClassFactory)
			: base(dtdLoader, defClassFactory)
        {
        }

        /// <summary>
        /// Loads the class data from the xml string provided
        /// </summary>
        /// <param name="xmlClassDef">The xml class definition string.
        /// You can use <code>new StreamReader("filename.xml").ReadToEnd()</code>
        /// to read a continuous string from a file.</param>
        /// <returns>Returns the class definition loaded</returns>
        public ClassDef LoadClass(string xmlClassDef)
        {
            if (xmlClassDef == null || xmlClassDef.Length == 0)
            {
                throw new ArgumentException("The application is unable to read the " +
                    "'classes' element from the XML class definitions file.  " +
                    "The definitions need one 'classes' root element and a 'class' " +
                    "element for each class that you are mapping.  XML elements are " +
                    "case-sensitive.");
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlClassDef);
            return LoadClass(doc.DocumentElement);
        }

        /// <summary>
        /// Loads the class data using the xml element provided
        /// </summary>
        /// <param name="classElement">The xml class element</param>
        /// <returns>Returns the class definition loaded</returns>
        public ClassDef LoadClass(XmlElement classElement)
        {
            return (ClassDef) this.Load(classElement);
        }

        /// <summary>
        /// Creates a class definition using the data loaded from the reader
        /// </summary>
        /// <returns>Returns a class definition</returns>
        protected override object Create()
        {
			ClassDef def = _defClassFactory.CreateClassDef(_AssemblyName, _ClassName, _PrimaryKeyDef, _PropDefCol, 
							 _KeyDefCol, _RelationshipDefCol, _UIDefCol);
			//ClassDef def =
			//    new ClassDef(_AssemblyName,_ClassName, _PrimaryKeyDef, _PropDefCol, 
			//                 _KeyDefCol, _RelationshipDefCol, _UIDefCol);
			if (_SuperClassDef != null)
            {
                def.SuperClassDef = _SuperClassDef;
            }
            if (_TableName != null && _TableName.Length > 0)
            {
                def.TableName = _TableName;
            }
            //def.SupportsSynchronising = _SupportsSynchronising;
            return def;
        }

        /// <summary>
        /// Loads all relevant data from the reader
        /// </summary>
        protected override void LoadFromReader()
        {
            _SuperClassDef = null;
            _reader.Read();
            LoadClassInfo();
            LoadTableName();
            //LoadSupportsSynchronisation();

            _reader.Read();

            List<string> keyDefXmls = new List<string>();
            List<string> propDefXmls = new List<string>();
            List<string> relationshipDefXmls = new List<string>();
            List<string> uiDefXmls = new List<string>();
            string superclassDescXML = null;
            string primaryKeDefXML = null;
            while (_reader.Name != "class")
            {
				switch (_reader.Name)
				{
					case "superClass":
						superclassDescXML = _reader.ReadOuterXml();
						break;
					case "property":
						propDefXmls.Add(_reader.ReadOuterXml());
						break;
					case "key":
						keyDefXmls.Add(_reader.ReadOuterXml());
						break;
					case "primaryKey":
						primaryKeDefXML = _reader.ReadOuterXml();
						break;
					case "relationship":
						relationshipDefXmls.Add(_reader.ReadOuterXml());
						break;
					case "ui":
						uiDefXmls.Add(_reader.ReadOuterXml());
						break;
					default:
						throw new InvalidXmlDefinitionException("The element '" +
							_reader.Name + "' is not a recognised class " +
							"definition element.  Ensure that you have the correct " +
							"spelling and capitalisation, or see the documentation " +
							"for available options.");
						break;
				}
            }

            LoadSuperClassDesc(superclassDescXML);
            LoadPropDefs(propDefXmls);
            LoadKeyDefs(keyDefXmls);
            LoadPrimaryKeyDef(primaryKeDefXML);
            LoadRelationshipDefs(relationshipDefXmls);
            LoadUIDefs(uiDefXmls);
        }

        /// <summary>
        /// Load the super-class data
        /// </summary>
        private void LoadSuperClassDesc(string xmlDef)
        {
            if (xmlDef != null)
        {
            XmlSuperClassLoader superClassLoader = new XmlSuperClassLoader(DtdLoader, _defClassFactory);
                _SuperClassDef = superClassLoader.LoadSuperClassDesc(xmlDef);
            }
        }

        /// <summary>
        /// Loads the relationship data
        /// </summary>
        private void LoadRelationshipDefs(List<string> xmlDefs)
        {
			_RelationshipDefCol = _defClassFactory.CreateRelationshipDefCol();
			//_RelationshipDefCol = new RelationshipDefCol();
			XmlRelationshipLoader relationshipLoader = new XmlRelationshipLoader(DtdLoader, _defClassFactory);
            foreach (string relDefXml in xmlDefs)
            {
                _RelationshipDefCol.Add(relationshipLoader.LoadRelationship(relDefXml, _PropDefCol));
            }
        }

        /// <summary>
        /// Loads the UIDef data
        /// </summary>
        private void LoadUIDefs(List<string> xmlDefs)
        {
			_UIDefCol = _defClassFactory.CreateUIDefCol();
			//_UIDefCol = new UIDefCol();
            XmlUILoader loader = new XmlUILoader(DtdLoader, _defClassFactory);
            foreach (string uiDefXml in xmlDefs)
            {
                _UIDefCol.Add(loader.LoadUIDef(uiDefXml));
            }
        }

        /// <summary>
        /// Loads the key definition data
        /// </summary>
        private void LoadKeyDefs(List<string> xmlDefs)
        {
			_KeyDefCol = _defClassFactory.CreateKeyDefCol();
			//_KeyDefCol = new KeyDefCol();
            XmlKeyLoader loader = new XmlKeyLoader(DtdLoader, _defClassFactory);
            foreach (string keyDefXml in xmlDefs)
            {
                _KeyDefCol.Add(loader.LoadKey(keyDefXml, _PropDefCol));
            }
        }

        /// <summary>
        /// Loads the primary key definition data
        /// </summary>
        private void LoadPrimaryKeyDef(string xmlDef)
        {
            if (xmlDef == null)
            {
                throw new InvalidXmlDefinitionException("Could not find a " +
                    "'primaryKey' element in the class definition for the class '" +
                    _ClassName + "'.  Each class definition requires a primary key " +
                    "definition, which is composed of one or more property definitions, " +
                    "implying that you will need at least one 'property' element as " +
                    "well.");
            }
			//_PrimaryKeyDef = new PrimaryKeyDef();
            XmlPrimaryKeyLoader primaryKeyLoader = new XmlPrimaryKeyLoader(DtdLoader, _defClassFactory);
            _PrimaryKeyDef = primaryKeyLoader.LoadPrimaryKey(xmlDef, _PropDefCol);
            if (_PrimaryKeyDef == null)
            {
                throw new InvalidXmlDefinitionException("There was an error loading " +
                    "the 'primaryKey' element in the class definition for the class '" +
                    _ClassName + "'.  Each class definition requires a primary key " +
                    "definition, which is composed of one or more property definitions, " +
                    "implying that you will need at least one 'property' element as " +
                    "well.");
            }
        }

        /// <summary>
        /// Loads the property definition data
        /// </summary>
        private void LoadPropDefs(List<string> xmlDefs)
        {
            if (xmlDefs.Count == 0)
            {
                throw new InvalidXmlDefinitionException(String.Format("No property " +
                    "definitions have been specified for the class definition of '{0}'. " +
                    "Each class requires at least one 'property' and 'primaryKey' " +
                    "element which define the mapping from the database table fields to " +
                    "properties in the class that is being mapped to.", _ClassName));
            }
			_PropDefCol = _defClassFactory.CreatePropDefCol();
            //_PropDefCol = new PropDefCol();
            XmlPropertyLoader propLoader = new XmlPropertyLoader(DtdLoader, _defClassFactory);
            foreach (string propDefXml in xmlDefs)
            {
                _PropDefCol.Add(propLoader.LoadProperty(propDefXml));
            }
            //			XmlNodeList xmlPropDefs = _lassElement.GetElementsByTagName("propertyDef");
            //			XmlPropertyLoader propLoader = new XmlPropertyLoader(_dtdPath);
            //			foreach (XmlNode xmlPropDef in xmlPropDefs) {
            //				_PropDefCol.Add(propLoader.LoadProperty(xmlPropDef.OuterXml));
            //			}
        }

        /// <summary>
        /// Loads the table name attribute
        /// </summary>
        private void LoadTableName()
        {
            _TableName = _reader.GetAttribute("table");
        }

        /// <summary>
        /// Loads the class type info data
        /// </summary>
        private void LoadClassInfo()
        {
            _ClassName = _reader.GetAttribute("name");
            _AssemblyName = _reader.GetAttribute("assembly");

            if (_AssemblyName == null || _AssemblyName.Length == 0)
            {
                string errorMessage = "No assembly name has been specified for the " +
                                      "class definition";
                if (_ClassName != null && _ClassName.Length > 0)
                {
                    errorMessage += " of '" + _ClassName + "'";
                }
                errorMessage += ". Within each 'class' element you need to set " +
                                "the 'assembly' attribute, which refers to the project or assembly " +
                                "that contains the class which is being mapped to.";
                throw new XmlException(errorMessage);
            }
            if (_ClassName == null || _ClassName.Length == 0)
            {
                throw new XmlException("No 'name' attribute has been specified for a " +
                   "'class' element.  The 'name' attribute indicates the name of the " +
                   "class to which a database table will be mapped.");
            }
        }

        ///// <summary>
        ///// Loads the attribute that indicates whether synchronisation is
        ///// supported
        ///// </summary>
        //private void LoadSupportsSynchronisation()
        //{
        //    string supportsSynch = _reader.GetAttribute("supportsSynchronising");
        //    try
        //    {
        //        _SupportsSynchronising = Convert.ToBoolean(supportsSynch);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidXmlDefinitionException(String.Format(
        //            "In the class definition for '{0}', the value provided for " +
        //            "the 'supportsSynchronising' attribute is not valid. The value " +
        //            "needs to be 'true' or 'false'.", _ClassName));
        //    }
        //}
    }
}
