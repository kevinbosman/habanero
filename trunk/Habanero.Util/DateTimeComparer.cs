using System;
using System.Collections;
using Habanero.Base;

namespace Habanero.Util
{
    /// <summary>
    /// Compares two business objects on the date-time property specified 
    /// in the constructor
    /// </summary>
    public class DateTimeComparer : IComparer
    {
        private readonly string _propName;

        /// <summary>
        /// Constructor to initialise a comparer, specifying the date-time property
        /// on which two business objects will be compared using the Compare()
        /// method
        /// </summary>
        /// <param name="propName">The integer property name on which two
        /// business objects will be compared</param>
        public DateTimeComparer(string propName)
        {
            _propName = propName;
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Compares two business objects on the date-time property specified in 
        /// the constructor
        /// </summary>
        /// <param name="x">The first object to compare</param>
        /// <param name="y">The second object to compare</param>
        /// <returns>Returns a negative number, zero or a positive number,
        /// depending on whether the first date is less, equal to or more
        /// than the second</returns>
        public int Compare(object x, object y)
        {
            IBusinessObject boLeft = (IBusinessObject) x;
            IBusinessObject boRight = (IBusinessObject) y;
            DateTime left;
            DateTime right;
            if (boLeft.GetPropertyValue(_propName) == null)
            {
                left = DateTime.MinValue;
            }
            else
            {
                left = (DateTime) boLeft.GetPropertyValue(_propName);
            }
            if (boRight.GetPropertyValue(_propName) == null)
            {
                right = DateTime.MinValue;
            }
            else
            {
                right = (DateTime) boRight.GetPropertyValue(_propName);
            }
            return left.CompareTo(right);
        }
    }
}