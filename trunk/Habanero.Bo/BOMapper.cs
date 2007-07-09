using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Habanero.Base.Exceptions;
using Habanero.Bo.ClassDefinition;
using Habanero.Base;
using log4net;

namespace Habanero.Bo
{
    /// <summary>
    /// Provides mapping to a business object
    /// </summary>
    public class BOMapper
    {
        private static readonly ILog log = LogManager.GetLogger("Habanero.Bo.BoMapper");
        private BusinessObject _businessObject;

        /// <summary>
        /// Constructor to initialise a new mapper
        /// </summary>
        /// <param name="bo">The business object to map</param>
        public BOMapper(BusinessObject bo)
        {
            _businessObject = bo;
        }

        ///// <summary>
        ///// Returns a property of the name specified
        ///// </summary>
        ///// <param name="propertyName">The property name</param>
        ///// <returns>Returns the BOProp object by that name</returns>
        //public BOProp GetProperty(string propertyName)
        //{
        //    return _businessObject.GetBOProp(propertyName);
        //}

        /// <summary>
        /// Returns the lookup list contents for the property specified
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>Returns the lookup list or an empty collection if
        /// not available</returns>
        public Dictionary<string, object> GetLookupList(string propertyName)
        {
            PropDef def = _businessObject.ClassDef.GetPropDef(propertyName);
            //return def.GetLookupList(_businessObject.GetDatabaseConnection());
            if (def.LookupListSource != null)
            {
                return def.LookupListSource.GetLookupList(_businessObject.GetDatabaseConnection());
            }
            else
            {
                return new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Returns the business object's interface mapper
        /// </summary>
        /// <returns>Returns the interface mapper</returns>
        public IUserInterfaceMapper GetUserInterfaceMapper()
        {
            return _businessObject.GetUserInterfaceMapper();
        }

        /// <summary>
        /// Returns the business object's interface mapper with the UIDefName
        /// specified
        /// </summary>
        /// <param name="uiDefName">The UIDefName</param>
        /// <returns>Returns the interface mapper</returns>
        public IUserInterfaceMapper GetUserInterfaceMapper(string uiDefName)
        {
            return _businessObject.GetUserInterfaceMapper(uiDefName);
        }

        /// <summary>
        /// Returns a property value in the form it will be displayed in a
        /// user interface (sometimes the underlying value may be stored
        /// differently, as in a lookup-list)
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>Returns the formatted object to display</returns>
        public object GetPropertyValueToDisplay(string propertyName)
        {
            if (propertyName.IndexOf(".") != -1)
            {
                //log.Debug("Prop with . found : " + propertyName);
                BusinessObject relatedBo = this._businessObject;
                string relationshipName = propertyName.Substring(0, propertyName.IndexOf("."));
                propertyName = propertyName.Remove(0, propertyName.IndexOf(".") + 1);
                if (relationshipName.IndexOf("|") != -1)
                {
                    //log.Debug("| found in relationship name :" + relationshipName);
                    ArrayList relNames = new ArrayList();
                    while (relationshipName.IndexOf("|") != -1)
                    {
                        string relName = relationshipName.Substring(0, relationshipName.IndexOf("|"));
                        relNames.Add(relName);
                        //log.Debug("Relationship name found : " + relName);
                        relationshipName = relationshipName.Remove(0, relationshipName.IndexOf("|") + 1);
                    }
                    relNames.Add(relationshipName);
                    BusinessObject oldBo = relatedBo;
                    int i = 0;
                    do
                    {
                        relatedBo = oldBo.Relationships.GetRelatedBusinessObject((String) relNames[i++]);
                    } while (relatedBo == null && i < relNames.Count);
                }
                else
                {
                    relatedBo = relatedBo.Relationships.GetRelatedBusinessObject(relationshipName);
                }
                if (relatedBo == null)
                {
                    return null;
                    //throw new HabaneroApplicationException("Unable to retrieve property " + propertyName + " from a business object of type " + this._businessObject.GetType().Name);
                }
                BOMapper relatedBoMapper = new BOMapper(relatedBo);
                return relatedBoMapper.GetPropertyValueToDisplay(propertyName);
            }
            else if (propertyName.IndexOf("-") != -1)
            {
                string virtualPropName = propertyName.Substring(1, propertyName.Length - 2);
                try
                {
                    PropertyInfo propInfo =
                        this._businessObject.GetType().GetProperty(virtualPropName,
                                                                     BindingFlags.Public | BindingFlags.Instance);
                    object propValue = propInfo.GetValue(this._businessObject, new object[] {});
                    return propValue;
                }

                catch (TargetInvocationException ex)
                {
                    log.Error("Error retrieving virtual property " + virtualPropName + " from object of type " +
                              this._businessObject.GetType().Name + Environment.NewLine +
                              ExceptionUtilities.GetExceptionString(ex.InnerException, 8, true));
                    throw ex.InnerException;
                }
            }
            else
            {
                return _businessObject.GetPropertyValueToDisplay(propertyName);
            }
        }

        /// <summary>
        /// Returns the lookup list related to the specified property in the
        /// class definition
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>Returns the lookup list or null if not available</returns>
        public ClassDef GetLookupListClassDef(string propertyName)
        {
            PropDef def = _businessObject.ClassDef.GetPropDef(propertyName);
            if (def.LookupListSource != null && def.LookupListSource.GetType() == typeof (DatabaseLookupListSource))
            {
                return ((DatabaseLookupListSource) def.LookupListSource).ClassDef;
            }
            return null;
        }
    }
}