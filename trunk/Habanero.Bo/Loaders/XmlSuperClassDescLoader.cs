using System;
using System.Xml;
using Habanero.Bo.ClassDefinition;
using Habanero.Base;
using Habanero.Util;

namespace Habanero.Bo.Loaders
{
    /// <summary>
    /// Loads super class information from xml data
    /// </summary>
    /// TODO ERIC - unclear what desc means - could rename to description
    /// or def or nothing
    public class XmlSuperClassDescLoader : XmlLoader
    {
        private ORMapping _orMapping;
        private string _className;
    	private string _assemblyName;


        /// <summary>
        /// Constructor to initialise a new loader with a dtd path
        /// </summary>
		/// <param name="dtdPath">The dtd path</param>
		/// <param name="defClassFactory">The factory for the definition classes</param>
		public XmlSuperClassDescLoader(string dtdPath, IDefClassFactory defClassFactory)
			: base(dtdPath, defClassFactory)
        {
        }

        /// <summary>
        /// Constructor to initialise a new loader
        /// </summary>
        public XmlSuperClassDescLoader()
        {
        }

        /// <summary>
        /// Loads super class information from the given xml string
        /// </summary>
        /// <param name="xmlSuperClassDesc">The xml string containing the
        /// super class definition</param>
        /// <returns>Returns the property rule object</returns>
        public SuperClassDesc LoadSuperClassDesc(string xmlSuperClassDesc)
        {
            return this.LoadSuperClassDesc(this.CreateXmlElement(xmlSuperClassDesc));
        }

        /// <summary>
        /// Loads super class information from the given xml element
        /// </summary>
        /// <param name="xmlSuperClassDesc">The xml element containing the
        /// super class definition</param>
        /// <returns>Returns the property rule object</returns>
        public SuperClassDesc LoadSuperClassDesc(XmlElement xmlSuperClassDesc)
        {
            return (SuperClassDesc) this.Load(xmlSuperClassDesc);
        }

        /// <summary>
        /// Creates a new SuperClassDesc object using the data that
        /// has been loaded for the object
        /// </summary>
        /// <returns>Returns a SuperClassDesc object</returns>
        protected override object Create()
        {
			return _defClassFactory.CreateSuperClassDesc(_assemblyName, _className, _orMapping);
			//return new SuperClassDesc(_assemblyName, _className, _orMapping);
		}

        /// <summary>
        /// Load the class data from the reader
        /// </summary>
        protected override void LoadFromReader()
        {
            _reader.Read();
            _className = _reader.GetAttribute("className");
            _assemblyName = _reader.GetAttribute("assemblyName");
			string orMappingType = _reader.GetAttribute("orMapping");
			try
            {
                _orMapping = (ORMapping)Enum.Parse(typeof(ORMapping), orMappingType);
            }
            catch (Exception ex)
            {
                throw new InvalidXmlDefinitionException(String.Format(
                    "The specified ORMapping type, '{0}', is not a valid inheritance " +
                    "type.  The valid options are ClassTableInheritance (the default), " +
                    "SingleTableInheritance and ConcreteTableInheritance.", orMappingType), ex);
            }

        }
    }
}